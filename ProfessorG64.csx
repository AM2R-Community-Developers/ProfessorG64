using System;
using System.IO;

// TODO: Ensure we're actually using 1.5.5

#region 15_to_17_To_16

ProfessorLog("Beginning bytecode update...");

EnsureDataLoaded();

string currentBytecodeVersion = Data?.GeneralInfo.BytecodeVersion.ToString();
string game_name = Data.GeneralInfo.Name.Content;

bool is13 = false;

if (!(Data.FORM.Chunks.ContainsKey("AGRP")))
{
    /*    is13 = true;
        ScriptMessage("Bytecode 13 type game detected. The upgrading of this game is experimental.");
        currentBytecodeVersion = "13";*/
    ScriptError("Bytecode 13 is not supported.");
    return;
}
if (Data?.GeneralInfo.BytecodeVersion == 14)
{
    ScriptError("Bytecode 14 is not supported.");
    return;
}

if (Data.IsVersionAtLeast(2, 3))
{
    ScriptError(game_name + "is GMS 2.3+ and is ineligible", "Ineligible");
    return;
}

if (!(Data.FORM.Chunks.ContainsKey("AGRP")))
{
    is13 = true;
    ScriptMessage("Bytecode 13 type game detected. The upgrading of this game is experimental.");
    currentBytecodeVersion = "13";
}


if ((Data?.GeneralInfo.BytecodeVersion == 14) || (Data?.GeneralInfo.BytecodeVersion == 15) || (is13 == true))
{
    // Removed pointless question, we always want to continue.
    ScriptMessage("Detected bytecode version " + currentBytecodeVersion);
    
    if (Data?.GeneralInfo.BytecodeVersion <= 14)
    {
        foreach (UndertaleCode code in Data.Code)
        {
            UndertaleCodeLocals locals = new UndertaleCodeLocals();
            locals.Name = code.Name;

            UndertaleCodeLocals.LocalVar argsLocal = new UndertaleCodeLocals.LocalVar();
            argsLocal.Name = Data.Strings.MakeString("arguments");
            argsLocal.Index = 0;

            locals.Locals.Add(argsLocal);

            code.LocalsCount = 1;
            Data.CodeLocals.Add(locals);
        }
    }
    if (!(Data.FORM.Chunks.ContainsKey("AGRP")))
    {
        Data.FORM.Chunks["AGRP"] = new UndertaleChunkAGRP();
        var previous = -1;
        var j = 0;
        for (var i = -1; i < Data.Sounds.Count - 1; i++)
        {
            UndertaleSound sound = Data.Sounds[i + 1];
            bool flagCompressed = sound.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsCompressed);
            bool flagEmbedded = sound.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsEmbedded);
            if (i == -1)
            {
                if (!flagCompressed && !flagEmbedded)
                {
                    sound.AudioID = -1;
                }
                else
                {
                    sound.AudioID = 0;
                    previous = 0;
                    j = 1;
                }
            }
            else
            {
                if (!flagCompressed && !flagEmbedded)
                    sound.AudioID = previous;
                else
                {
                    sound.AudioID = j;
                    previous = j;
                    j++;
                }
            }
        }
        foreach (UndertaleSound sound in Data.Sounds)
        {
            if ((sound.AudioID >= 0) && (sound.AudioID < Data.EmbeddedAudio.Count))
            {
                sound.AudioFile = Data.EmbeddedAudio[sound.AudioID];
            }
            sound.GroupID = 0;
        }
        Data.GeneralInfo.Build = 1804;
        var newProductID = new byte[] { 0xBA, 0x5E, 0xBA, 0x11, 0xBA, 0xDD, 0x06, 0x60, 0xBE, 0xEF, 0xED, 0xBA, 0x0B, 0xAB, 0xBA, 0xBE };
        Data.FORM.EXTN.productIdData.Add(newProductID);
        Data.Options.Constants.Clear();
        Data.Options.Constants.Add(new UndertaleOptions.Constant() { Name = Data.Strings.MakeString("@@SleepMargin"), Value = Data.Strings.MakeString(1.ToString()) });
        Data.Options.Constants.Add(new UndertaleOptions.Constant() { Name = Data.Strings.MakeString("@@DrawColour"), Value = Data.Strings.MakeString(0xFFFFFFFF.ToString()) });
    }
    Data.FORM.Chunks["LANG"] = new UndertaleChunkLANG();
    Data.FORM.LANG.Object = new UndertaleLanguage();
    Data.FORM.Chunks["GLOB"] = new UndertaleChunkGLOB();
    String[] order = { "GEN8", "OPTN", "LANG", "EXTN", "SOND", "AGRP", "SPRT", "BGND", "PATH", "SCPT", "GLOB", "SHDR", "FONT", "TMLN", "OBJT", "ROOM", "DAFL", "TPAG", "CODE", "VARI", "FUNC", "STRG", "TXTR", "AUDO" };
    Dictionary<string, UndertaleChunk> newChunks = new Dictionary<string, UndertaleChunk>();
    foreach (String name in order)
        newChunks[name] = Data.FORM.Chunks[name];
    Data.FORM.Chunks = newChunks;
    Data.GeneralInfo.BytecodeVersion = 16;
    ScriptMessage("Upgraded from " + currentBytecodeVersion + " to 16 successfully. Save the game to apply the changes.");
}
else if (Data?.GeneralInfo.BytecodeVersion == 17)
{
    if (!ScriptQuestion("Downgrade bytecode from 17 to 16?"))
    {
        ScriptMessage("Cancelled.");
        return;
    }
    Data.GeneralInfo.BytecodeVersion = 16;
    if (Data.FORM.Chunks.ContainsKey("TGIN"))
        Data.FORM.Chunks.Remove("TGIN");
    Data.SetGMS2Version(2);
    ScriptMessage("Downgraded from 17 to 16 successfully. Save the game to apply the changes.");
}
else if (Data?.GeneralInfo.BytecodeVersion == 16)
{
    ScriptError("This is already bytecode 16.", "Error");
    return;
}
else
{
    string error = @"This game is not bytecode 13, 
14, 15, 16, or 17, and is not made in GameMaker 2.3
or greater. Please report this game to Grossley#2869
on Discord and provide the name of the game, where
you obtained it from, and additionally send the
data.win file of the game." + @"

Game and version: '" + Data.GeneralInfo.ToString() + "'";
    ScriptError(error, "Unknown game error");
    SetUMTConsoleText(error);
    SetFinishedMessage(false);
    return;
}

ProfessorLog("Finished updating bytecode.");

#endregion



#region Patch out removed functions

ProfessorLog("Patching out removed functions...");

// TODO: We need a real replacement for this system, it's vital for custom localizations!
AddMissing("font_replace");
AddMissing("immersion_stop");
AddMissing("immersion_play_effect");

ProfessorLog("Finished patching removed functions.");

#endregion



#region Import replacement scripts

ProfessorLog("Importing replacement scripts...");

foreach (var file in Directory.GetFiles("Replacement Scripts"))
{
    ImportGMLFile(file);
}

ProfessorLog("Replacement scripts imported.");

#endregion



#region Fix decompiler bugs

ProfessorLog("Fixing decompiler bugs...");



ProfessorLog("Decompiler bugs fixed.");

#endregion



#region Bugfixes

ProfessorLog("Fixing game bugs...");



ProfessorLog("Game bugs fixed.");

#endregion



#region Performance improvements


ProfessorLog("Applying performance improvements...");



ProfessorLog("Performance improvements applied.");

#endregion



#region Misc. modifications

ProfessorLog("Applying misc. modifications...");

ReplaceTextInGML("gml_Object_oControl_Create_0", "V1.5.5", "V1.6.0 B1");

// TODO: Credits

ProfessorLog("Misc. modifications applied.");

#endregion






#region Methods

void AddMissing(string name)
{
    UndertaleScript scr = new UndertaleScript();
    scr.Name = Data.Strings.MakeString(name);
    scr.Code = Data.Code.ByName(name);
    Data.Scripts.Add(scr);
}

void ProfessorLog(string message)
{
    ScriptMessage("[ProfessorG64]" + message);
}

// TODO: Add a folder for GML files to use for larger replacements instead of being inline

void ReplaceGML(string name, string replacement)
{
    var gml = Data.Code.ByName(name);
    // var gml = GetDecompiledText(data.Name.Content);
    gml.ReplaceGML(replacement, Data);
}


#endregion