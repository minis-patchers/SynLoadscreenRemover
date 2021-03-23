using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using SynLoadScreenRemover.Types;

namespace SynLoadScreenRemover
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynLSR.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var JOBJ = JObject.Parse(File.ReadAllText(Path.Combine(state.ExtraSettingsDataPath, "settings.json"))).ToObject<Settings>();
            var stat = state.PatchMod.Statics.AddNew("None");
            foreach (var ls in state.LoadOrder.PriorityOrder.LoadScreen().WinningOverrides())
            {
                var nls = state.PatchMod.LoadScreens.GetOrAddAsOverride(ls);
                nls.LoadingScreenNif.SetTo(stat);
                if (JOBJ.RemoveLoreText)
                {
                    nls.Description = "";
                }
            }
        }
    }
}