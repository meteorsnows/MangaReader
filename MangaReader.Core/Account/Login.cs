﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MangaReader.Core.Entity;
using MangaReader.Manga;
using MangaReader.Services;

namespace MangaReader.Account
{
  public abstract class Login : Entity
  {
    public static Guid Type { get { return Guid.Empty; } }

    public static Guid Manga { get { return Guid.Empty; } }

    public bool IsLogined
    {
      get { return isLogined; }
      set
      {
        isLogined = value;
        OnLoginStateChanged(value);
      }
    }

    public event EventHandler<bool> LoginStateChanged;

    public virtual string Name { get; set; }

    public virtual string Password { get; set; }

    public virtual bool CanLogin { get { return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password); } }

    public virtual Uri MainUri { get; set; }

    public virtual Uri LogoutUri { get; set; }

    public virtual Uri BookmarksUri { get; set; }

    /// <summary>
    /// Указатель блокировки клиента файла.
    /// </summary>
    protected internal object ClientLock { get; set; }

    /// <summary>
    /// Клиент с куками авторизованного пользователя.
    /// </summary>
    protected internal CookieClient Client
    {
      get { return this.client ?? (this.client = new CookieClient() {BaseAddress = MainUri.ToString()}); }
    }

    private CookieClient client;
    private bool isLogined;

    public virtual async Task<bool> DoLogin()
    {
      return false;
    }

    public virtual async Task<bool> Logout()
    {
      IsLogined = false;
      using (TimedLock.Lock(ClientLock))
      {
        await Page.GetPageAsync(LogoutUri, Client);
      }
      return true;
    }

    public async Task<List<Mangas>> GetBookmarks()
    {
      Log.Add(string.Format("Start load bookmarks from '{0}'.", this.MainUri));
      var bookmarks = await DownloadBookmarks();
      Log.Add(string.Format("Finish load bookmarks from '{0}'.", this.MainUri));
      return bookmarks;
    }

    protected virtual async Task<List<Mangas>> DownloadBookmarks()
    {
      return new List<Mangas>();
    }

    public static Login Create(Guid manga)
    {
      var baseClass = typeof(Login);
      // TODO: с учетом подключаемых либ - искать надо везде.
      var types = baseClass.Assembly.GetTypes()
        .Where(type => type.IsSubclassOf(baseClass));

      Login login = null;
      foreach (var type in types)
      {
        if (Equals(type.MangaProperty(), manga))
          login = (Login)Activator.CreateInstance(type);
      }
      return login;
    }

    protected Login()
    {
      this.ClientLock = new object();
    }

    protected virtual void OnLoginStateChanged(bool e)
    {
      LoginStateChanged?.Invoke(this, e);
    }
  }
}
