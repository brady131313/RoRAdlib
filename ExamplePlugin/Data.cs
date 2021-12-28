using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using R2API;
using BepInEx.Configuration;

namespace RoRAdlib
{
    class Data : MonoBehaviour
    {
        static public List<ItemConfig> allItemConfigs = new List<ItemConfig>();
        static public List<ItemConfig> allEquipmentConfigs = new List<ItemConfig>();

        static public void PopulateItemCatalogues(ConfigFile config)
        {
            Log.LogInfo("Populating Equipment Catalogue");
            foreach (var equipment in EquipmentCatalog.equipmentDefs)
            {
                if (equipment.nameToken != null && equipment.nameToken != "")
                {
                    allEquipmentConfigs.Add(new ItemConfig(config, equipment));
                }
            }

            Log.LogInfo("Populating Item Catalogue");
            foreach (var item in ItemCatalog.itemDefs)
            {
                if (item.nameToken != null && item.nameToken != "")
                {
                    allItemConfigs.Add(new ItemConfig(config, item));
                }
            }
        }

        static public void OverrideItemNames()
        {
            Log.LogInfo("Overriding item names");

            foreach (var equipment in allEquipmentConfigs)
            {
                equipment.OverrideNames();
            }

            foreach (var item in allItemConfigs)
            {
                item.OverrideNames();
            }
        }

        static protected string sanitizeName(string name)
        {
            var sanitized = name.Trim().Replace("'", "");
            if (sanitized.Contains("<"))
            {
                var start = sanitized.IndexOf(">") + 1;
                var end = sanitized.IndexOf("<", start);
                return sanitized.Substring(start, end - start);
            } else
            {
                return sanitized;
            }
        }

        public class ItemConfig
        {
            public (string, ConfigEntry<string>) nameToken;
            public (string, ConfigEntry<string>) pickupToken;
            public (string, ConfigEntry<string>) descToken;

            public ItemConfig(ConfigFile config, ItemDef item)
            {
                string name = Language.GetString(item.nameToken, Language.currentLanguage.name);
                string section = "Item - " + sanitizeName(name);

                nameToken = (item.nameToken, config.Bind(section, "Name", ""));

                if (item.pickupToken != null)
                {
                    pickupToken = (item.pickupToken, config.Bind(section, "Pickup", ""));
                }

                if (item.descriptionToken != null)
                {
                    descToken = (item.descriptionToken, config.Bind(section, "Description", ""));
                }
            }

            public ItemConfig(ConfigFile config, EquipmentDef equipment)
            {

                string name = Language.GetString(equipment.nameToken, Language.currentLanguage.name);
                string section = "Equipment - " + sanitizeName(name);

                nameToken = (equipment.nameToken, config.Bind(section, "Name", ""));

                if (equipment.pickupToken != null)
                {
                    pickupToken = (equipment.pickupToken, config.Bind(section, "Pickup", ""));
                }

                if (equipment.descriptionToken != null)
                {
                    descToken = (equipment.descriptionToken, config.Bind(section, "Description", ""));
                }
            }

            public void OverrideNames()
            {
                if (nameToken.Item2.Value != "")
                {
                    LanguageAPI.Add(nameToken.Item1, nameToken.Item2.Value);
                }
            }
        }
    }
}
