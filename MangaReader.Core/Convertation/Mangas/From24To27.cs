﻿using System;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga.Acomic;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Mangas
{
  public class From24To27 : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && 
        Version.CompareTo(ConfigStorage.Instance.DatabaseConfig.Version) > 0 && 
        process.Version.CompareTo(Version) >= 0;
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      process.Percent = 0;
      var acomics = Repository.Get<Acomics>().ToList();
      foreach (var acomic in acomics)
      {
        Getter.UpdateContentType(acomic);
        process.Percent += 100.0 / acomics.Count;
      }
      acomics.SaveAll();
    }

    public From24To27()
    {
      this.Version = new Version(1, 27, 5584);
    }
  }
}