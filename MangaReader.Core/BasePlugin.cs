﻿using System;
using System.Linq;
using System.Reflection;
using MangaReader.Core.Services;

namespace MangaReader.Core
{
  /// <summary>
  /// Базовая реализация плагина для снижения дублирования кода.
  /// </summary>
  /// <remarks>Не обязательна к использованию.</remarks>
  public abstract class BasePlugin : IPlugin
  {
    public virtual string Name { get { return this.MangaType.Name; } }
    public abstract string ShortName { get; }
    public abstract Assembly Assembly { get; }
    public abstract Guid MangaGuid { get; }
    public abstract Type MangaType { get; }
    public abstract Type LoginType { get; }

    public virtual MangaSetting GetSettings()
    {
#warning will be reworked -- entities returned with closed session
      using (var context = NHibernate.Repository.GetEntityContext("Just get settings"))
        return context.Get<MangaSetting>().Single(m => m.Manga == this.MangaGuid);
    }

    public abstract ISiteParser GetParser();
    public abstract HistoryType HistoryType { get; }
  }
}