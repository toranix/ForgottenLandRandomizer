﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using ForgottenLandRandomizer.Types;
using ForgottenLandRandomizer.Util;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ForgottenLandRandomizer
{
    public partial class MainForm : Form
    {
        public static string ExeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        const int DEE_SCALING = 5;

        static string[] StoryStages = new string[]
        {
            "Level1Stage1",
            "Level1Stage2",
            "Level1Stage3",
            "Level1Stage4",
            "Level1Stage5",
            "Level2Stage1",
            "Level2Stage2",
            "Level2Stage3",
            "Level2Stage4",
            "Level2Stage5",
            "Level3Stage1",
            "Level3Stage2",
            "Level3Stage3",
            "Level3Stage4",
            "Level3Stage5",
            "Level4Stage1",
            "Level4Stage2",
            "Level4Stage3",
            "Level4Stage4",
            "Level4Stage5",
            "Level5Stage1",
            "Level5Stage2",
            "Level5Stage3",
            "Level5Stage4",
            "Level5Stage5",
            "Level6Stage1",
            "Level6Stage2",
            "Level6Stage3",
            "Level6Stage4",
            "Level6Stage5"
        };

        static string[] CopyAbilities = new string[]
        {
            "0x3", "0x4", "0x5", "0x6", "0x7", "0x8", "0x9", "0xA", "0xB", "0xC", "0xD", "0xE"
        };

        public MainForm()
        {
            InitializeComponent();
            if (File.Exists(ExeDir + "\\Config.xml"))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(ExeDir + "\\Config.xml");
                romfsPath.Text = xml["Config"]["RomFS"].InnerText;
            }
            else
            {
                XmlDocument xml = new XmlDocument();
                xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", ""));

                XmlElement root = xml.CreateElement("Config");
                XmlElement romfs = xml.CreateElement("RomFS");
                romfs.InnerText = romfsPath.Text;
                root.AppendChild(romfs);

                xml.AppendChild(root);

                xml.Save(ExeDir + "\\Config.xml");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", ""));

            XmlElement root = xml.CreateElement("Config");
            XmlElement romfs = xml.CreateElement("RomFS");
            romfs.InnerText = romfsPath.Text;
            root.AppendChild(romfs);

            xml.AppendChild(root);

            xml.Save(ExeDir + "\\Config.xml");
        }

        private void openRomfs_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog open = new CommonOpenFileDialog();
            open.IsFolderPicker = true;
            if (open.ShowDialog() == CommonFileDialogResult.Ok)
            {
                romfsPath.Text = open.FileName;
            }
        }

        private void romfsHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                  "This program requires Kirby and the Forgotten Land's RomFS to work properly." +
                "\nClick the elipses (...) button and select the directory containing the game's RomFS, or enter it yourself in the box." +
                "\nThe RomFS path provided will be saved in Config.xml when the program is closed and autofilled in when the program opens." +
                "\n" +
                "\nNote that not all files in the RomFS are needed to randomize the game. The necessary files are:" +
                "\n   - basil/Scn.bin" +
                "\n   - basil/Seq.bin" +
                "\n   - yaml/Scn/Step/Map/WmapData.bin" +
                "\n" +
                "\nHover over anything in the program for more information about the option.",
                this.Text,
                MessageBoxButtons.OK);
        }

        private void randStoryStages_CheckChanged(object sender, EventArgs e)
        {
            randStoryStagesPanel.Visible = randStoryStages.Checked;
        }

        private void randCopyAbilities_CheckChanged(object sender, EventArgs e)
        {
            randCopyAbilitiesPanel.Visible = randCopyAbilities.Checked;
        }

        private void randomize_Click(object sender, EventArgs e)
        {
            Random rng = new Random();
            Dictionary<string, bool> isTouchFile = new Dictionary<string, bool>();
            isTouchFile["Scn"] = isTouchFile["Seq"] = isTouchFile["WmapData"] = false;

            Console.WriteLine("Creating seed...");
            int rngSeed;
            if (seed.Text != "")
            {
                if (uint.TryParse(seed.Text, out uint parsedSeed))
                    rngSeed = (int)parsedSeed;
                else if (uint.TryParse(seed.Text, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint hexSeed))
                    rngSeed = (int)hexSeed;
                else
                    rngSeed = BitConverter.ToInt32(HashCalculator.Calculate(seed.Text), 0);
            }
            else
                rngSeed = rng.Next(int.MinValue, int.MaxValue);

            Console.WriteLine("Seed: " + (uint)rngSeed);

            string outDir = ExeDir + $"\\OutFiles\\seed_{seed.Text}";

            if (!File.Exists(romfsPath.Text + "\\basil\\Seq.bin"))
            {
                MessageBox.Show($"Error: Could not find the path \"{romfsPath.Text}\\basil\\Seq.bin\".", this.Text, MessageBoxButtons.OK);
                return;
            }

            if (!File.Exists(romfsPath.Text + "\\basil\\Scn.bin"))
            {
                MessageBox.Show($"Error: Could not find the path \"{romfsPath.Text}\\basil\\Scn.bin\".", this.Text, MessageBoxButtons.OK);
                return;
            }

            if (!File.Exists(romfsPath.Text + "\\yaml\\Scn\\Step\\Map\\WmapData.bin"))
            {
                MessageBox.Show($"Error: Could not find the path \"{romfsPath.Text}\\yaml\\Scn\\Step\\Map\\WmapData.bin\".", this.Text, MessageBoxButtons.OK);
                return;
            }

            Console.WriteLine("Loading Seq.bin Mint binary...");
            Archive mintSeq;
            using (EndianBinaryReader reader = new EndianBinaryReader(new FileStream(romfsPath.Text + "\\basil\\Seq.bin", FileMode.Open, FileAccess.Read)))
            {
                mintSeq = new Archive(reader);
            }

            Console.WriteLine("Loading Scn.bin Mint binary...");
            Archive mintScn;
            using (EndianBinaryReader reader = new EndianBinaryReader(new FileStream(romfsPath.Text + "\\basil\\Scn.bin", FileMode.Open, FileAccess.Read)))
            {
                mintScn = new Archive(reader);
            }

            Console.WriteLine("Loading WmapData.bin YAML binary...");
            Yaml yamlWmapData = new Yaml(romfsPath.Text + "\\yaml\\Scn\\Step\\Map\\WmapData.bin");

            if (randStoryStages.Checked)
            {
                isTouchFile["Scn"] = isTouchFile["Seq"] = isTouchFile["WmapData"] = true;
                rng = new Random(rngSeed);
                string[] shuffledStoryStages = StoryStages.OrderBy(x => rng.Next(0, StoryStages.Length)).ToArray();
                MintClass storyStageKind = mintSeq.Scripts["Storage.File.StoryStageKind"].Classes[0];
                storyStageKind.Constants.Clear();
                storyStageKind.Constants.Add(new MintClass.MintConstant("Invalid", 0x0));
                storyStageKind.Constants.Add(new MintClass.MintConstant("Level0Stage1", 0x1));
                for (int i = 0x0; i < shuffledStoryStages.Length; i++)
                {
                    storyStageKind.Constants.Add(new MintClass.MintConstant(shuffledStoryStages[i], (uint)i + 0x2));
                    if (shuffledStoryStages[i][11] == '5')
                        yamlWmapData.Root["BossStageUnlockCount"][shuffledStoryStages[i].Substring(0,6)] = new Yaml.Data(Yaml.Type.Int, scaleDees.Checked ? i*DEE_SCALING : 0);
                }
                yamlWmapData.Root["BossStageUnlockCount"]["Level6"] = new Yaml.Data(Yaml.Type.Int, 30 * DEE_SCALING);
                storyStageKind.Constants.Add(new MintClass.MintConstant("Level6Stage6", 0x20));
                storyStageKind.Constants.Add(new MintClass.MintConstant("Level7Stage1", 0x21));
                storyStageKind.Constants.Add(new MintClass.MintConstant("TERM", 0x22));

                for (int i = 0; i < yamlWmapData.Root.Length; i++)
                {
                    if (yamlWmapData.Root.Type == Yaml.Type.Hash)
                        PrintYaml(yamlWmapData.Root[yamlWmapData.Root.Key(i)], yamlWmapData.Root.Key(i), 0);
                    else if (yamlWmapData.Root.Type == Yaml.Type.Array)
                        PrintYaml(yamlWmapData.Root[i], i.ToString(), 0);
                }

                // Modify stage unlock scripts to support shuffled stage order
                string script;
                string[] newScript;
                script = "Scn.Step.Info.StageList.Wmap.StageList.Panel";
                newScript = File.ReadAllLines(ExeDir + "\\MintScripts\\" + script + ".mints");
                mintScn.Scripts[script] = new MintScript(newScript, new byte[] { 7, 0, 2, 0 });
                
                script = "Scn.Step.SceneChangeCtrl";
                newScript = File.ReadAllLines(ExeDir + "\\MintScripts\\" + script + ".mints");
                mintScn.Scripts[script] = new MintScript(newScript, new byte[] { 7, 0, 2, 0 });   
            }
            if (randCopyAbilities.Checked)
            {
                isTouchFile["Scn"] = true;
                if (randCopyAbilitiesSeeded.Checked)
                {
                    rng = new Random(rngSeed);
                    string[] shuffledCopyAbilities = CopyAbilities.OrderBy(x => rng.Next(0, CopyAbilities.Length)).ToArray();
                    Console.WriteLine(string.Join(", ", CopyAbilities));
                    Console.WriteLine(string.Join(", ", shuffledCopyAbilities));
                    string[] gainScript = File.ReadAllLines(ExeDir + "\\MintScripts\\Scn.Step.Actor.Kirby.KirbyBuildUtil.mints");
                    string[] discardScript = File.ReadAllLines(ExeDir + "\\MintScripts\\Scn.Step.Actor.Kirby.DiscardUtil.mints");
                    string[] evolveScript = File.ReadAllLines(ExeDir + "\\MintScripts\\Scn.Step.Actor.Gimmick.Mannequin.KirbyStateEvolution.mints");
                    string[] mannequinScript = File.ReadAllLines(ExeDir + "\\MintScripts\\Scn.Step.Actor.Gimmick.Mannequin.KirbyStateMannequinSet.mints");
                    string[] stateCopyScript = File.ReadAllLines(ExeDir + "\\MintScripts\\Scn.Step.Actor.Kirby.Common.StateCopy.mints");
                    for (int i = 0; i < shuffledCopyAbilities.Length; i++)
                    {
                        string tag = $"%ABILITY_{i}%";
                        for (int line = 0; line < gainScript.Length; line++)
                        {
                            if (gainScript[line].Contains(tag))
                            {
                                gainScript[line] = gainScript[line].Replace(tag, shuffledCopyAbilities[i]);
                            }
                        }
                        for (int line = 0; line < evolveScript.Length; line++)
                        {
                            if (evolveScript[line].Contains(tag))
                            {
                                evolveScript[line] = evolveScript[line].Replace(tag, shuffledCopyAbilities[i]);
                            }
                        }
                        for (int line = 0; line < mannequinScript.Length; line++)
                        {
                            if (mannequinScript[line].Contains(tag))
                            {
                                mannequinScript[line] = mannequinScript[line].Replace(tag, shuffledCopyAbilities[i]);
                            }
                        }
                        for (int line = 0; line < discardScript.Length; line++)
                        {
                            if (discardScript[line].Contains(tag))
                            {
                                discardScript[line] = discardScript[line].Replace(tag, CopyAbilities[Array.FindIndex(shuffledCopyAbilities, s => s == CopyAbilities[i])]);
                            }
                        }
                        for (int line = 0; line < stateCopyScript.Length; line++)
                        {
                            if (stateCopyScript[line].Contains(tag))
                            {
                                stateCopyScript[line] = stateCopyScript[line].Replace(tag, CopyAbilities[Array.FindIndex(shuffledCopyAbilities, s => s == CopyAbilities[i])]);
                            }
                        }
                    }

                    mintScn.Scripts["Scn.Step.Actor.Kirby.KirbyBuildUtil"] = new MintScript(gainScript, new byte[] { 7, 0, 2, 0 });
                    mintScn.Scripts["Scn.Step.Actor.Kirby.DiscardUtil"] = new MintScript(discardScript, new byte[] { 7, 0, 2, 0 });
                    mintScn.Scripts["Scn.Step.Actor.Gimmick.Mannequin.KirbyStateEvolution"] = new MintScript(evolveScript, new byte[] { 7, 0, 2, 0 });
                    mintScn.Scripts["Scn.Step.Actor.Gimmick.Mannequin.KirbyStateMannequinSet"] = new MintScript(mannequinScript, new byte[] { 7, 0, 2, 0 });
                    mintScn.Scripts["Scn.Step.Actor.Kirby.Common.StateCopy"] = new MintScript(stateCopyScript, new byte[] { 7, 0, 2, 0 });
                } else if (randCopyAbilitiesChaos.Checked)
                {
                    string buildScript = "Scn.Step.Actor.Kirby.KirbyBuildUtil";
                    string discardScript = "Scn.Step.Actor.Kirby.DiscardUtil";
                    string[] newBuildScript = File.ReadAllLines(ExeDir + "\\MintScripts\\CHAOS" + buildScript + ".mints");
                    string[] newDiscardScript = File.ReadAllLines(ExeDir + "\\MintScripts\\CHAOS" + discardScript + ".mints");
                    mintScn.Scripts[buildScript] = new MintScript(newBuildScript, new byte[] { 7, 0, 2, 0 });
                    mintScn.Scripts[discardScript] = new MintScript(newDiscardScript, new byte[] { 7, 0, 2, 0 });
                }
            }

            if ((isTouchFile["Scn"] || isTouchFile["Seq"]) && !Directory.Exists(outDir + "\\basil"))
                Directory.CreateDirectory(outDir + "\\basil");

            if (isTouchFile["WmapData"] && !Directory.Exists(outDir + "\\yaml\\Scn\\Step\\Map"))
                Directory.CreateDirectory(outDir + "\\yaml\\Scn\\Step\\Map");

            if (randStoryStages.Checked && !Directory.Exists(outDir + "\\lyt\\Wmap\\StageList"))
            {
                Directory.CreateDirectory(outDir + "\\lyt\\Wmap\\StageList");
                File.Copy(ExeDir + "\\Assets\\Parts.arc.cmp", outDir + "\\lyt\\Wmap\\StageList\\Parts.arc.cmp", true);
            }

            if (isTouchFile["Seq"])
            {
                Console.WriteLine("Saving Seq Mint binary...");
                mintSeq.Write(outDir + "\\basil\\Seq.bin");
            }

            if (isTouchFile["Scn"])
            {
                Console.WriteLine("Saving Scn Mint binary...");
                mintScn.Write(outDir + "\\basil\\Scn.bin");
            }

            if (isTouchFile["WmapData"])
            {
                Console.WriteLine("Saving WmapData YAML binary...");
                yamlWmapData.Write(outDir + "\\yaml\\Scn\\Step\\Map\\WmapData.bin");
            }

            MessageBox.Show($"Successfully randomized!\nMod-ready files saved to \"{outDir}\"", this.Text, MessageBoxButtons.OK);
        }


        //-- Yaml tests --

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Binary files|*.bin";
            if (open.ShowDialog() == DialogResult.OK)
            {
                Yaml yaml = new Yaml(open.FileName);

                for (int i = 0; i < yaml.Root.Length; i++)
                {
                    if (yaml.Root.Type == Yaml.Type.Hash)
                        PrintYaml(yaml.Root[yaml.Root.Key(i)], yaml.Root.Key(i), 0);
                    else if (yaml.Root.Type == Yaml.Type.Array)
                        PrintYaml(yaml.Root[i], i.ToString(), 0);
                }
            }
        }

        void PrintYaml(Yaml.Data data, string name, int tabIndex)
        {
            string text = "";
            for (int i = 0; i < tabIndex; i++)
                text += "\t";
            text += $"- [{data.Type}] {name}";
            switch (data.Type)
            {
                case Yaml.Type.Int:
                    text += ": " + data.ToInt();
                    break;
                case Yaml.Type.Float:
                    text += ": " + data.ToFloat();
                    break;
                case Yaml.Type.Bool:
                    text += ": " + data.ToBool();
                    break;
                case Yaml.Type.String:
                    text += ": \"" + data.ToString() + "\"";
                    break;
                case Yaml.Type.Array:
                case Yaml.Type.Hash:
                    text += ": " + data.Length + " children";
                    break;
            }

            Console.WriteLine(text);
            for (int i = 0; i < data.Length; i++)
            {
                if (data.Type == Yaml.Type.Hash)
                    PrintYaml(data[data.Key(i)], data.Key(i), tabIndex + 1);
                else if (data.Type == Yaml.Type.Array)
                    PrintYaml(data[i], i.ToString(), tabIndex + 1);
            }
        }
    }
}
