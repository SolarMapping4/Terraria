﻿// Decompiled with JetBrains decompiler
// Type: Terraria.World.Generation.GenModShape
// Assembly: TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null
// MVID: C7ED7F12-DBD9-42C5-B3E5-7642F0F95B55
// Assembly location: E:\Steam\SteamApps\common\Terraria\TerrariaServer.exe

namespace Terraria.World.Generation
{
  public abstract class GenModShape : GenShape
  {
    protected ShapeData _data;

    public GenModShape(ShapeData data)
    {
      this._data = data;
    }
  }
}
