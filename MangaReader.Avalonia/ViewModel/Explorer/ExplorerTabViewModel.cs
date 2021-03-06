﻿using System.Threading.Tasks;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class ExplorerTabViewModel : ViewModelBase
  {
    private string name;
    private int priority;

    public string Name
    {
      get { return name; }
      set { RaiseAndSetIfChanged(ref name, value); }
    }

    public int Priority
    {
      get { return priority; }
      set { RaiseAndSetIfChanged(ref priority, value); }
    }

    public virtual Task OnSelected(ExplorerTabViewModel previousModel)
    {
      return Task.CompletedTask;
    }

    public virtual Task OnUnselected(ExplorerTabViewModel newModel)
    {
      return Task.CompletedTask;
    }
  }
}