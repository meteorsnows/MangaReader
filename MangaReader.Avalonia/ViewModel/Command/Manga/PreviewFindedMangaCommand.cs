﻿using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class PreviewFindedMangaCommand : BaseCommand
  {
    public override async Task Execute(object parameter)
    {
      var model = parameter as MangaSearchViewModel;
      if (model == null)
        return;

      var manga = await Mangas.Create(model.Uri).ConfigureAwait(true);
      if (manga == null)
        return;

      manga.Cover = model.Cover;
      await manga.Refresh().ConfigureAwait(true);
      if (await manga.IsValid().ConfigureAwait(true))
      {
        var explorer = ExplorerViewModel.Instance;
        var searchTab = new MangaModel(manga);
        explorer.Tabs.Add(searchTab);
        explorer.SelectedTab = searchTab;
      }
    }

    public PreviewFindedMangaCommand()
    {
      this.Name = "Preview";
    }
  }
}