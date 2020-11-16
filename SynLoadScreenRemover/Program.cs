using Newtonsoft.Json.Linq;
using System.IO;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using SynLoadScreenRemover.Types;

namespace SynLoadScreenRemover
{
    class Program
    {
        public static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                new UserPreferences() {
                ActionsForEmptyArgs = new RunDefaultPatcher
                {
                    IdentifyingModKey = "SynLSR.esp",
                    TargetRelease = GameRelease.SkyrimSE
                }
            });
        }
        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var JOBJ = JObject.Parse(File.ReadAllText(Path.Combine(state.ExtraSettingsDataPath, "settings.json"))).ToObject<config>();
            var stat = state.PatchMod.Statics.AddNew("None");
            foreach(var ls in state.LoadOrder.PriorityOrder.OnlyEnabled().LoadScreen().WinningOverrides()) {
                var nls = state.PatchMod.LoadScreens.GetOrAddAsOverride(ls);
                nls.LoadingScreenNif = stat.FormKey;
                if(JOBJ.RemoveLoreText) {
                    nls.Description = "";
                }
            }
        }
    }
}