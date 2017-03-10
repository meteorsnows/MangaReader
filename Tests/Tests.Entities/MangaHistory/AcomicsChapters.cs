﻿using System;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.MangaHistory
{
  [TestClass]
  public class AcomicsChapters
  {
    [TestMethod]
    public void CreateWithHistoryAndGetLastVolume()
    {
      var manga = Builder.CreateAcomics();
      manga.Uri = new Uri("http://acomics.ru/~ma3");

      var volumeUri = new Uri("http://acomics.ru/~ma3/935");
      var chapterUri = new Uri("http://acomics.ru/~ma3/1129");
      manga.AddHistory(chapterUri);
      manga.Save();

      manga.Refresh();
      manga.Name = Guid.NewGuid().ToString();
      manga.Save();

      var chapter = new Chapter(chapterUri) { Pages = { new MangaPage(new Uri("http://acomics.ru/~ma3/1130"), null, 1) } };
      var volume = new Volume() { Uri = volumeUri, Chapters = { chapter } };

      var newChapters = History.GetItemsWithoutHistory(volume);
      Assert.AreEqual(1, newChapters.Count);
    }

    [TestMethod]
    public void CreateWithHistoryAndGetNotLastVolume()
    {
      var manga = Builder.CreateAcomics();
      manga.Uri = new Uri("http://acomics.ru/~ma3");

      var volumeUri = new Uri("http://acomics.ru/~ma3/777");
      var chapterUri = new Uri("http://acomics.ru/~ma3/793");
      manga.AddHistory(chapterUri);
      manga.Save();

      manga.Refresh();
      manga.Name = Guid.NewGuid().ToString();
      manga.Save();

      var chapter = new Chapter(chapterUri) { Pages = { new MangaPage(new Uri("http://acomics.ru/~ma3/794"), null, 1) } };
      var volume = new Volume() { Uri = volumeUri, Chapters = { chapter } };

      var newChapters = History.GetItemsWithoutHistory(volume);
      Assert.AreEqual(1, newChapters.Count);
    }
  }
}