using Android.Content;
using System;
using System.Collections.Generic;

namespace SlideMessaging.Droid.Helpers
{
    class FavoritesHelper
    {
        public static List<string> GetFavorites(Context context)
        {
            var sharedPref = GetFavoritePreferences(context);

            var editor = sharedPref.Edit();

            var favorites = new List<string>();
            favorites.AddRange(sharedPref.All.Keys);

            return favorites;
        }

        public static void AddFavorite(Context context, string threadId)
        {
            if (!IsFavorite(context, threadId))
            {
                var sharedPref = GetFavoritePreferences(context);

                var editor = sharedPref.Edit();

                try
                {
                    editor.PutString(threadId, String.Empty);
                    editor.Commit();
                }
                catch { }
            }
        }

        public static void RemoveFavorite(Context context, string threadId)
        {
            if (IsFavorite(context, threadId))
            {
                var sharedPref = GetFavoritePreferences(context);

                var editor = sharedPref.Edit();

                try
                {
                    editor.Remove(threadId);
                    editor.Commit();
                }
                catch { }
            }
        }

        public static bool IsFavorite(Context context, string threadId)
        {
            var sharedPref = GetFavoritePreferences(context);
            var favorites = new List<string>();
            favorites.AddRange(sharedPref.All.Keys);

            var exists = favorites.Contains(threadId);

            return exists;
        }

        private static ISharedPreferences GetFavoritePreferences(Context context)
        {
            var sharedPref = context.GetSharedPreferences(
                    context.GetString(Resource.String.favorites_key), FileCreationMode.Private);

            return sharedPref;
        }
    }
}