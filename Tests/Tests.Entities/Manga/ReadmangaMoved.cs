﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grouple;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class ReadmangaMoved : TestClass
  {
    [Test]
    public async Task CreateWithHistoryAndMove()
    {
      var model = new MangaReader.Core.Services.LibraryViewModel();
      using (var context = Repository.GetEntityContext())
      {
        foreach (var remove in (await context.Get<IManga>().ToListAsync().ConfigureAwait(false)).Where(m => m.ServerName.Contains("btooom")))
          await model.Remove(remove).ConfigureAwait(false);

        var manga = await Builder.CreateReadmanga().ConfigureAwait(false);
        var readmangaUri = new Uri("http://readmanga.me/btoom");
        manga.Uri = readmangaUri;
        manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(new Uri("http://readmanga.me/btoom/vol1/1?mature=1")));
        await context.Save(manga).ConfigureAwait(false);

        manga = await context.Get<Readmanga>().FirstOrDefaultAsync(m => m.Id == manga.Id).ConfigureAwait(false);
        await manga.Refresh().ConfigureAwait(false);

        // Если сайт больше не редиректит, осталась возможность редиректа вручную в клиенте.
        if (manga.Uri == readmangaUri)
          manga.Uri = new Uri("http://mintmanga.com/btooom_");

        await context.Save(manga).ConfigureAwait(false);

        var volume = new Volume();
        volume.Container.Add(new Chapter(new Uri("http://mintmanga.com/btooom_/vol1/1?mature=1"), string.Empty));

        var chartersNotInHistory = History.GetItemsWithoutHistory(volume);
        Assert.AreEqual(0, chartersNotInHistory.Count);
      }
    }
  }
}
