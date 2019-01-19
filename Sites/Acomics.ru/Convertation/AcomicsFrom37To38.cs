﻿using System;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Acomics.Convertation
{
  public class AcomicsFrom37To38 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Acomics>().GetSettings();
        if (setting != null && setting.MainUri == null)
        {
          setting.MainUri = new Uri("http://acomics.ru/");
          setting.Login.MainUri = setting.MainUri;
          context.Save(setting);
        }
      }
    }

    public AcomicsFrom37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}