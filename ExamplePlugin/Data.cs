using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using R2API;
using BepInEx.Configuration;

namespace RoRAdlib
{
    class Data : MonoBehaviour
    {
        static public Dictionary<DataCategory, List<ConfigEntry<string>>> allData = new();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public", Justification = "<Pending>")]
        static public void PopulateItemCatalogues(ConfigFile config)
        {
            var tokenGroups = new Dictionary<DataCategory, List<string>>();
            foreach (var item in Language.currentLanguage.stringsByToken)
            {
                var splitKey = item.Key.Split('_');
                var section = CastDataCategory(splitKey[0]);
                var suffix = CastDataSuffix(splitKey[splitKey.Length - 1]);


                if (section != DataCategory.Other && suffix != DataSuffix.Other)
                {
                    if (!tokenGroups.ContainsKey(section))
                    {
                        tokenGroups.Add(section, new List<string>());
                    }
                    tokenGroups[section].Add(item.Key);
                }
            }

            foreach (var group in tokenGroups)
            {
                group.Value.Sort();
                allData.Add(group.Key, new List<ConfigEntry<string>>());

                foreach (var key in group.Value)
                {
                    var defaultVal = Language.currentLanguage.stringsByToken[key];

                    // Get the root and see if it has a name suffix
                    // If it does then use that for the section heading instead of category
                    var root = StripSuffix(key);
                    var rootWithName = root + "_NAME";
                    if (Language.currentLanguage.stringsByToken.ContainsKey(rootWithName))
                    {
                        var groupRaw = Language.currentLanguage.stringsByToken[rootWithName];
                        var groupSanitized = group.Key.ToString() + " :: " + SanitizeName(groupRaw);

                        allData[group.Key].Add(config.Bind(groupSanitized, key, defaultVal));
                    }
                    else
                    {
                        allData[group.Key].Add(config.Bind(group.Key.ToString(), key, defaultVal));
                    }

                }
            }
        }

        static public void OverrideItemNames()
        {
            Log.LogInfo("Overriding item names");

            foreach (var section in allData)
            {
                foreach (var itemConfig in section.Value)
                {
                    LanguageAPI.Add(itemConfig.Definition.Key, itemConfig.Value);
                }
            }
        }

        static protected string StripSuffix(string name)
        {
            foreach (var suffix in Enum.GetValues(typeof(DataSuffix)))
            {
                var suffixStr = "_" + suffix.ToString().ToUpper();
                if (name.Contains(suffixStr))
                {
                    return name.Replace(suffixStr, "");
                }
            }

            return name;
        }

        static protected string SanitizeName(string name)
        {
            var sanitized = name.Trim().Replace("'", "");
            if (sanitized.Contains("<"))
            {
                var start = sanitized.IndexOf(">") + 1;
                var end = sanitized.IndexOf("<", start);
                return sanitized.Substring(start, end - start);
            }
            else
            {
                return sanitized;
            }
        }

        public enum DataCategory
        {
            Item,
            Equipment,
            Shrine,
            Lunar,
            Newt,
            Bazaar,
            Other
        }

        static protected DataCategory CastDataCategory(string category)
        {
            switch (category)
            {
                case "ITEM":
                    return DataCategory.Item;
                case "EQUIPMENT":
                    return DataCategory.Equipment;
                case "SHRINE":
                    return DataCategory.Shrine;
                case "LUNAR":
                    return DataCategory.Lunar;
                case "NEWT":
                    return DataCategory.Newt;
                case "BAZAAR":
                    return DataCategory.Bazaar;
                default:
                    return DataCategory.Other;
            }
        }

        public enum DataSuffix
        {
            Name,
            Pickup,
            Desc,
            Lore,
            Context,
            Message,
            Other
        }

        static protected DataSuffix CastDataSuffix(string suffix)
        {
            switch (suffix)
            {
                case "NAME":
                    return DataSuffix.Name;
                case "PICKUP":
                    return DataSuffix.Pickup;
                case "DESC":
                    return DataSuffix.Desc;
                case "LORE":
                    return DataSuffix.Lore;
                case "CONTEXT":
                    return DataSuffix.Context;
                case "MESSAGE":
                    return DataSuffix.Message;
                default:
                    return DataSuffix.Other;
            }
        }


    }
}
