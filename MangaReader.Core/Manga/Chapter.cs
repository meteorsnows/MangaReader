﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga
{
  /// <summary>
  /// Глава.
  /// </summary>
  [DebuggerDisplay("{Number} {Name}")]
  public class Chapter : DownloadableContainerImpl<MangaPage>
  {
    #region Свойства

    /// <summary>
    /// Том.
    /// </summary>
    public Volume Volume { get; set; }

    /// <summary>
    /// Номер главы.
    /// </summary>
    public double Number { get; set; }

    #endregion

    public override string Folder
    {
      get { return FolderNamingStrategies.GetNamingStrategy(Volume?.Manga ?? Manga).FormateChapterFolder(this); }
    }

    public bool OnlyUpdate { get; set; }

    #region Методы

    /// <summary>
    /// Скачать главу.
    /// </summary>
    /// <param name="downloadFolder">Папка для файлов.</param>
    public override async Task Download(string downloadFolder = null)
    {
      await DownloadManager.CheckPause().ConfigureAwait(false);
      if (!DirectoryHelpers.ValidateSettingPath(downloadFolder))
        throw new DirectoryNotFoundException($"Попытка скачивания в папку {downloadFolder}, папка не существует.");

      var chapterFolder = Path.Combine(downloadFolder, DirectoryHelpers.RemoveInvalidCharsFromName(this.Folder));

      if (this.Container == null || !this.Container.Any())
        await this.UpdatePages().ConfigureAwait(false);

      this.InDownloading = this.Container.ToList();
      if (this.OnlyUpdate)
      {
        await DownloadManager.CheckPause().ConfigureAwait(false);
        this.InDownloading = History.GetItemsWithoutHistory(this);
      }

      try
      {
        await DownloadManager.CheckPause().ConfigureAwait(false);
        if (!Directory.Exists(chapterFolder))
          Directory.CreateDirectory(chapterFolder);

        var pTasks = this.InDownloading.Select(page => page.Download(chapterFolder).LogException(string.Empty, $"Не удалось скачать изображение {page.ImageLink} со страницы {page.Uri}"));
        await Task.WhenAll(pTasks.ToArray()).ConfigureAwait(false);
      }
      catch (AggregateException ae)
      {
        foreach (var ex in ae.Flatten().InnerExceptions)
          Log.Exception(ex, $"Не удалось скачать главу {Name} по ссылке {Uri}");
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, $"Не удалось скачать главу {Name} по ссылке {Uri}");
      }
    }

    /// <summary>
    /// Обновить список страниц.
    /// </summary>
    /// <remarks>Каждая конкретная глава сама забьет коллекцию this.Container.</remarks>
    protected virtual async Task UpdatePages()
    {
      var parser = this.Volume?.Manga?.Parser ?? this.Manga.Parser;
      await parser.UpdatePages(this).ConfigureAwait(false);

      if (this.Container == null)
        throw new ArgumentNullException(nameof(Container));
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на главу.</param>
    /// <param name="name">Название главы.</param>
    public Chapter(Uri uri, string name) : this()
    {
      this.Uri = uri;
      this.Name = name;
    }

    protected Chapter()
    {

    }

    #endregion
  }
}
