﻿// Decompiled with JetBrains decompiler
// Type: Terraria.Program
// Assembly: TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null
// MVID: C7ED7F12-DBD9-42C5-B3E5-7642F0F95B55
// Assembly location: E:\Steam\SteamApps\common\Terraria\TerrariaServer.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.Social;

namespace Terraria
{
  public static class Program
  {
    public static Dictionary<string, string> LaunchParameters = new Dictionary<string, string>();
    private static int ThingsToLoad = 0;
    private static int ThingsLoaded = 0;
    public static bool LoadedEverything = false;
    public const bool IsServer = true;
    public static IntPtr JitForcedMethodCache;

    public static float LoadedPercentage
    {
      get
      {
        if (Program.ThingsToLoad == 0)
          return 1f;
        return (float) Program.ThingsLoaded / (float) Program.ThingsToLoad;
      }
    }

    public static void StartForceLoad()
    {
      if (!Main.SkipAssemblyLoad)
        ThreadPool.QueueUserWorkItem(new WaitCallback(Program.ForceLoadThread));
      else
        Program.LoadedEverything = true;
    }

    public static void ForceLoadThread(object ThreadContext)
    {
      Program.ForceLoadAssembly(Assembly.GetExecutingAssembly(), true);
      Program.LoadedEverything = true;
    }

    private static void ForceJITOnAssembly(Assembly assembly)
    {
      foreach (Type type in assembly.GetTypes())
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
          if (!method.IsAbstract && !method.ContainsGenericParameters && method.GetMethodBody() != null)
            RuntimeHelpers.PrepareMethod(method.MethodHandle);
        }
        ++Program.ThingsLoaded;
      }
    }

    private static void ForceStaticInitializers(Assembly assembly)
    {
      foreach (Type type in assembly.GetTypes())
      {
        if (!type.IsGenericType)
          RuntimeHelpers.RunClassConstructor(type.TypeHandle);
      }
    }

    private static void ForceLoadAssembly(Assembly assembly, bool initializeStaticMembers)
    {
      Program.ThingsToLoad = assembly.GetTypes().Length;
      Program.ForceJITOnAssembly(assembly);
      if (!initializeStaticMembers)
        return;
      Program.ForceStaticInitializers(assembly);
    }

    private static void ForceLoadAssembly(string name, bool initializeStaticMembers)
    {
      Assembly assembly = (Assembly) null;
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      for (int index = 0; index < assemblies.Length; ++index)
      {
        if (assemblies[index].GetName().Name.Equals(name))
        {
          assembly = assemblies[index];
          break;
        }
      }
      if (assembly == (Assembly) null)
        assembly = Assembly.Load(name);
      Program.ForceLoadAssembly(assembly, initializeStaticMembers);
    }

    public static void LaunchGame(string[] args)
    {
      Program.LaunchParameters = Utils.ParseArguements(args);
      ThreadPool.SetMinThreads(8, 8);
      LanguageManager.Instance.SetLanguage("English");
      using (Main game = new Main())
      {
        try
        {
          SocialAPI.Initialize(new SocialMode?());
          LaunchInitializer.LoadParameters(game);
          Main.OnEnginePreload += (Action) (() => Program.StartForceLoad());
          game.DedServ();
        }
        catch (Exception ex)
        {
          Program.DisplayException(ex);
        }
      }
    }

    private static void DisplayException(Exception e)
    {
      try
      {
        using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", true))
        {
          streamWriter.WriteLine((object) DateTime.Now);
          streamWriter.WriteLine((object) e);
          streamWriter.WriteLine("");
        }
        Console.WriteLine(Language.GetTextValue("Error.ServerCrash"), (object) DateTime.Now, (object) e);
      }
      catch
      {
      }
    }
  }
}
