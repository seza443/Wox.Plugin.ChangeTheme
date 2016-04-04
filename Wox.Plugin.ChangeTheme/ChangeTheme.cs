using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.ChangeTheme
{
    /// <summary>
    /// List the theme of the user and switch the active theme with the selected one
    /// author: seza443
    /// </summary>
    public class ChangeTheme : IPlugin
    {
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            string queryFilter = query.Search;
            List<Theme> themesNames = null;
            if (queryFilter != null && !String.IsNullOrEmpty(queryFilter))
            {
                themesNames = ThemeManager.getFilteredThemesNames(queryFilter);
            }
            else
            {
                themesNames = ThemeManager.getAllThemesNames();
            }
            string currentThemeName = ThemeManager.GetActiveThemeName();
            List<Result> results = themesNames.Select(p => new Result()
            {
                Title = p.Name,
                SubTitle = currentThemeName.Equals(p.Name) ? "Currently active" : p.Path,
                IcoPath = "Images\\Entypo_e79a(0)_64.png",  //relative path to your plugin directory
                Action = e =>
                {
                    // after user select the item
                    if (!currentThemeName.Equals(p.Name))
                    {
                        ThemeManager.SwitchTheme(p.Path);
                        return true;
                    }
                    else {
                        // return false to tell Wox don't hide query window, otherwise Wox will hide it automatically
                        return false;
                    }
                }
            }).ToList();

            return results;
        }
    }
}
