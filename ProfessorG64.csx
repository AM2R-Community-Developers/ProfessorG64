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

AddMissing("font_replace");
AddMissing("immersion_stop");
AddMissing("immersion_play_effect");

ProfessorLog("Finished patching removed functions.");

#endregion



#region Import new scripts

ProfessorLog("Importing new scripts...");

ReplaceGMLFromFolder("New Scripts");

ProfessorLog("New scripts imported.");

#endregion



#region Import replacement scripts

ProfessorLog("Importing replacement scripts...");

ReplaceGMLFromFolder("Replacement Scripts");

ReplaceGMLFromFolder("Replacement Scripts/ds_list memory leak cleanup");

ReplaceGMLFromFolder("Replacement Scripts/Monster_Beam_Collision_Fixes");

ProfessorLog("Replacement scripts imported.");

#endregion



#region Fix individual decompiler bugs

ProfessorLog("Fixing individual decompiler bugs...");



ProfessorLog("Decompiler bugs fixed.");

#endregion



#region Bugfixes

ProfessorLog("Fixing individual game bugs...");

// Patches GUI surface displaying at incorrect scales.
// TODO: Could this use application surface values for sizing instead?
// TODO: This is busted with fullscreen still. Just draw it over the application_surface next, ugh
ReplaceTextInGML("gml_Object_oControl_Draw_64",
                 "draw_surface_ext(gui_surface, (displayx - d[0]), (displayy - d[1]), display_scale, display_scale, 0, -1, 1)",
                 "draw_surface_ext(gui_surface, (displayx - d[0]) / display_scale, (displayy - d[1]) / display_scale, 1, 1, 0, -1, 1)");

// Patches IGT surface displaying at incorrect scales. Same as above.
ReplaceTextInGML("gml_Object_oIGT_Draw_64",
                 "draw_surface_ext(igt_surface, (oControl.displayx - d[0]), (oControl.displayy - d[1]), oControl.display_scale, oControl.display_scale, 0, -1, 1)",
                 "draw_surface_ext(igt_surface, (oControl.displayx - d[0]) / oControl.display_scale, (oControl.displayy - d[1]) / oControl.display_scale, 1, 1, 0, -1, 1)");

ProfessorLog("Patched HUD scale bug.");

// Globally replace oCharacter.surf with global.characterSurf.
// TODO: This does NOT fix the nvidia speedbooster trail issue!
ReplaceTextInGML("gml_Object_oCharacterTrail_Create_0",
                 "oCharacter.surf", 
				 "global.characterSurf");

ReplaceTextInGML("gml_Script_draw_character_from_surface",
                 "oCharacter.surf", 
				 "global.characterSurf");

ReplaceTextInGML("gml_Script_draw_character_to_surface",
                 "oCharacter.surf", 
				 "global.characterSurf");

ReplaceTextInGML("gml_Script_characterCreateEvent",
                 "surf = surface_create(64, 64)",
				 "global.characterSurf = surface_create(64, 64)");
				 
ReplaceTextInGML("gml_Object_oCharacter_Destroy_0",
                 "surface_free(surf)",
				 "surface_free(global.characterSurf)");

ProfessorLog("Patched oCharacter.surf references with a global variable.");

// Fix floor slope mask sizes.
// TODO: Check ceiling slopes?
SetSpriteMargins("sSlope1", 0, 16, 16, 0);
SetSpriteMargins("sSlope2", 0, 16, 16, 0);
SetSpriteMargins("sSlopeFL1", 0, 16, 32, 0);
SetSpriteMargins("sSlopeFL2", 0, 16, 32, 0);

ProfessorLog("Patched slope mask sizes.");

// Fix font loading.
ReplaceTextInGML("gml_Object_oControl_Create_0",
                 "font_replace(global.fontGUI, ",
                 "global.fontGUI = font_add(");

ReplaceTextInGML("gml_Object_oControl_Create_0",
                 "font_replace(global.fontSubScr, ",
                 "global.fontSubScr = font_add(");

ReplaceTextInGML("gml_Object_oControl_Create_0",
                 "font_replace(global.fontGUI2, ",
                 "global.fontGUI2 = font_add(");

ReplaceTextInGML("gml_Object_oControl_Create_0",
                 "font_replace(global.fontMenuSmall, ",
                 "global.fontMenuSmall = font_add(");

ReplaceTextInGML("gml_Object_oControl_Create_0",
                 "font_replace(global.fontMenuTiny, ",
                 "global.fontMenuTiny = font_add(");

ReplaceTextInGML("gml_Object_oControl_Create_0",
                 "font_replace(global.fontMenuSmall2, ",
                 "global.fontMenuSmall2 = font_add(");

ProfessorLog("Patched external font loading.");

ProfessorLog("Game bugs fixed.");

#endregion



#region Performance improvements

ProfessorLog("Applying performance improvements...");



ProfessorLog("Performance improvements applied.");

#endregion



#region Misc. modifications

ProfessorLog("Applying misc. modifications...");

ReplaceTextInGML("gml_Object_oControl_Create_0", "V1.5.5", "V1.6.0 B1");

// TODO: Ensure credits cover everybody!

/*
    FORMATTING KEY:
    ; = linebreak marker
    = = split into left/right strings
    / = centered name
    * = centered header (gets custom display color)
*/
ReplaceTextInGML("gml_Object_oCreditsText_Create_0",

                 "*Continued Revisions;;*Development;Gatordile=Lojemiru;/Alex 'Wanderer' Mack;;*Programming;\") + global.monsterStr) + \"3D=Scooterboot;/Craig Kostelecky;/milesthenerd;;*Art Lead;/Dannon 'Shmegleskimo' Yates;;*Art;ShirtyScarab=Cooper Garvin;/Chris 'Messianic' Oliveira;/ChloePlz;;*Debug;Miepee=EODTex;/Esteban 'DruidVorse' Criado;/Verneri 'Naatiska' Viljanen;/Electrix;;*Localization;Imsu=Diegomg;m3Zz=LPCaiser;Miepee=unknown;fedprod=ReNext;LetsPlayNintendoITA=SadNES cITy e Vecna;Atver=Gponys;DarkEspeon=Vectrex28;R3VOWOOD=Ritinha;LiveLM=pMega0n;peachflavored=Katherine_S2003;PanHooHa=realgard;Mister Bond=joe_urahara;RippeR1692=LudvigNG;/Andréas;;*Special Thanks;Banjo=King Bore;Reaku the Crate=Grom PE;Sylandro=TheKhaosDemon;Iwantdevil=PixHammer;GaptGlitch=Nokbient;Nanassshy=kitronmacaron;/Jean-Samuel Pelletier;/Japanese Community",

                 "*Continued Revisions;;*Development;Gatordile=Lojemiru;/Alex 'Wanderer' Mack;;*Programming;\") + global.monsterStr) + \"3D=Scooterboot;/Craig Kostelecky;/milesthenerd;;*Art Lead;/Dannon 'Shmegleskimo' Yates;;*Art;ShirtyScarab=Cooper Garvin;/Chris 'Messianic' Oliveira;/ChloePlz;;*Debug;Miepee=EODTex;/Esteban 'DruidVorse' Criado;/Verneri 'Naatiska' Viljanen;/Electrix;;*Localization;Imsu=Diegomg;m3Zz=LPCaiser;Miepee=unknown;fedprod=ReNext;LetsPlayNintendoITA=SadNES cITy e Vecna;Atver=Gponys;DarkEspeon=Vectrex28;R3VOWOOD=Ritinha;LiveLM=pMega0n;peachflavored=Katherine_S2003;PanHooHa=realgard;Mister Bond=joe_urahara;RippeR1692=LudvigNG;/Andréas;;*Special Thanks;Banjo=King Bore;Reaku the Crate=Grom PE;Sylandro=TheKhaosDemon;Iwantdevil=PixHammer;GaptGlitch=Nokbient;Nanassshy=kitronmacaron;/Jean-Samuel Pelletier;/Japanese Community;/UndertaleModTool Team;/Johnny on Flame;Abyss=Haihaa;/Xander McCord");

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
    ScriptMessage("[ProfessorG64] " + message);
}

void ReplaceGML(string name, string replacement)
{
    var gml = Data.Code.ByName(name);
    gml.ReplaceGML(replacement, Data);
}

void ReplaceGMLFromFolder(string name)
{
    foreach (var file in Directory.GetFiles(name))
    {
        ImportGMLFile(file);
    }
}

// This is EXTREMELY slow. Only use if absolutely necessary!
void GlobalReplace(string text, string replacement)
{
    foreach (var code in Data.Code)
    {    
        ReplaceTextInGML(code.Name.Content, text, replacement, true, false);
    }
}

void SetSpriteMargins(string name, int left, int right, int bottom, int top)
{
    var sprite = Data.Sprites.ByName(name);
    sprite.MarginLeft = left;
    sprite.MarginRight = right;
    sprite.MarginBottom = bottom;
    sprite.MarginTop = top;
}

#endregion
