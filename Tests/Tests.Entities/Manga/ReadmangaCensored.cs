﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class ReadmangaCensored : TestClass
  {
    private Grouple.Parser parser = new Grouple.Parser();

    // Not censored
    // http://readmanga.me/black_butler/vol3/10?mature=1
    // Censored
    // http://readmanga.me/school_teacher/vol2/10?mature=1

    [Test]
    public void NotCensoredReadmanga()
    {
      var manga = Get(@"http://readmanga.me/black_butler/vol3/10?mature=1");
      var chapter = manga.Volumes[2].Chapters[0];
      Grouple.Parser.UpdatePages(chapter as Grouple.Chapter);
      Assert.IsTrue(chapter.Pages[0].ImageLink.IsAbsoluteUri);
    }

    [Test]
    public void CensoredReadmanga()
    {
      var manga = Get(@"http://readmanga.me/school_teacher/vol2/10?mature=1");
      var chapter = manga.Volumes[1].Chapters[5];
      Grouple.Parser.UpdatePages(chapter as Grouple.Chapter);
      Assert.IsTrue(chapter.Pages[0].ImageLink.IsAbsoluteUri);
    }

    private IManga Get(string url)
    {
      var manga = Mangas.Create(new Uri(url));
      parser.UpdateContent(manga);
      return manga;
    }
  }
}
