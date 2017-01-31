﻿// Decompiled with JetBrains decompiler
// Type: Terraria.IO.FileMetadata
// Assembly: TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null
// MVID: C7ED7F12-DBD9-42C5-B3E5-7642F0F95B55
// Assembly location: E:\Steam\SteamApps\common\Terraria\TerrariaServer.exe

using System;
using System.IO;

namespace Terraria.IO
{
  public class FileMetadata
  {
    public const ulong MAGIC_NUMBER = 27981915666277746;
    public const int SIZE = 20;
    public FileType Type;
    public uint Revision;
    public bool IsFavorite;

    private FileMetadata()
    {
    }

    public void Write(BinaryWriter writer)
    {
      writer.Write((ulong) (27981915666277746L | (long) this.Type << 56));
      writer.Write(this.Revision);
      writer.Write((ulong) (this.IsFavorite.ToInt() & 1));
    }

    public void IncrementAndWrite(BinaryWriter writer)
    {
      ++this.Revision;
      this.Write(writer);
    }

    public static FileMetadata FromCurrentSettings(FileType type)
    {
      return new FileMetadata()
      {
        Type = type,
        Revision = 0,
        IsFavorite = false
      };
    }

    public static FileMetadata Read(BinaryReader reader, FileType expectedType)
    {
      FileMetadata fileMetadata = new FileMetadata();
      fileMetadata.Read(reader);
      if (fileMetadata.Type != expectedType)
        throw new FileFormatException("Expected type \"" + Enum.GetName(typeof (FileType), (object) expectedType) + "\" but found \"" + Enum.GetName(typeof (FileType), (object) fileMetadata.Type) + "\".");
      return fileMetadata;
    }

    private void Read(BinaryReader reader)
    {
      ulong num1 = reader.ReadUInt64();
      if (((long) num1 & 72057594037927935L) != 27981915666277746L)
        throw new FileFormatException("Expected Re-Logic file format.");
      byte num2 = (byte) (num1 >> 56 & (ulong) byte.MaxValue);
      FileType fileType = FileType.None;
      FileType[] values = (FileType[]) Enum.GetValues(typeof (FileType));
      for (int index = 0; index < values.Length; ++index)
      {
        if (values[index] == (FileType) num2)
        {
          fileType = values[index];
          break;
        }
      }
      if (fileType == FileType.None)
        throw new FileFormatException("Found invalid file type.");
      this.Type = fileType;
      this.Revision = reader.ReadUInt32();
      this.IsFavorite = ((long) reader.ReadUInt64() & 1L) == 1L;
    }
  }
}
