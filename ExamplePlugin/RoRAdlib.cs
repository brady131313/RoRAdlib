using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace RoRAdlib
{
    //This attribute specifies that we have a dependency on R2API, as we're using it to add our item to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(R2API.R2API.PluginGUID)]
	
	//This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
	
	//We will be using 3 modules from R2API: ItemAPI to add our item, ItemDropAPI to have our item drop ingame, and LanguageAPI to add our language tokens.
    [R2APISubmoduleDependency(nameof(LanguageAPI))] 
	
	//This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class RoRAdlib : BaseUnityPlugin
	{
        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "BradyPierce";
        public const string PluginName = "RoRAdlib";
        public const string PluginVersion = "1.0.0";

        

		//The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);

            RoR2Application.onLoad += () => Data.PopulateItemCatalogues(Config);
            RoR2Application.onLoad += Data.OverrideItemNames;

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        //This function adds the tokens from the item using LanguageAPI, the comments in here are a style guide, but is very opiniated. Make your own judgements!
        private void AddTokens()
        {
            //The Name should be self explanatory
            LanguageAPI.Add("EXAMPLE_CLOAKONKILL_NAME", "Cuthroat's Garb");

            //The Pickup is the short text that appears when you first pick this up. This text should be short and to the point, numbers are generally ommited.
            LanguageAPI.Add("EXAMPLE_CLOAKONKILL_PICKUP", "Chance to cloak on kill");

            //The Description is where you put the actual numbers and give an advanced description.
            LanguageAPI.Add("EXAMPLE_CLOAKONKILL_DESC", "Whenever you <style=cIsDamage>kill an enemy</style>, you have a <style=cIsUtility>5%</style> chance to cloak for <style=cIsUtility>4s</style> <style=cStack>(+1s per stack)</style.");
            
            //The Lore is, well, flavor. You can write pretty much whatever you want here.
            LanguageAPI.Add("EXAMPLE_CLOAKONKILL_LORE", "Those who visit in the night are either praying for a favour, or preying on a neighbour.");
        }

    }
}
