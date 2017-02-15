﻿using MangaReader.Core.Update;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class AppUpdateCommand : LibraryBaseCommand
  {
    public override void Execute(object parameter)
    {
      Updater.StartUpdate();
    }

    public AppUpdateCommand()
    {
      this.Name = Strings.Library_CheckUpdate;
      this.Icon = "pack://application:,,,/Icons/Main/update_app.png";
    }
  }
}