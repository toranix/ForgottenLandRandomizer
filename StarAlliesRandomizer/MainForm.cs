using System;
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
using StarAlliesRandomizer.Types;
using StarAlliesRandomizer.Util;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace StarAlliesRandomizer
{
    public partial class MainForm : Form
    {
        public static string ExeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

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
                "\n   - yaml/Scn/Step/Map/WmapData.bin (Unimplemented currently)" +
                "\n" +
                "\nHover over anything in the program for more information about the option.",
                this.Text,
                MessageBoxButtons.OK);
        }

        /*
        private void randCopyAbilities_CheckedChanged(object sender, EventArgs e)
        {
            randAbilityPanel.Visible = randCopyAbilities.Checked;
        }
        */

        private void randomize_Click(object sender, EventArgs e)
        {
            Random rng = new Random();

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

            string outDir = ExeDir + $"\\OutFiles\\seed_{(uint)rngSeed}";

            if (!File.Exists(romfsPath.Text + "\\basil\\Seq.bin"))
            {
                MessageBox.Show($"Error: Could not find the path \"{romfsPath.Text}\\basil\\Seq.bin\".", this.Text, MessageBoxButtons.OK);
                return;
            }

            Console.WriteLine("Loading Seq.bin Mint binary...");
            Archive mintStep;
            using (EndianBinaryReader reader = new EndianBinaryReader(new FileStream(romfsPath.Text + "\\basil\\Seq.bin", FileMode.Open, FileAccess.Read)))
            {
                mintStep = new Archive(reader);
            }

            rng = new Random(rngSeed);
            if (randStoryStages.Checked)
            {
                /*
                if (atkRandDamage.Checked)
                {
                    int limitDamage = int.MaxValue;
                    if (doLimitDmg.Checked)
                        limitDamage = (int)limitDmg.Value;

                    var attackPowerScripts = mintStep.Scripts.Where(x => ((x.Key.StartsWith("Scn.Step.Hero.") && x.Key.EndsWith(".AttackPower")) || (atkRandEnemy.Checked && x.Key.EndsWith(".AttackPower"))) );
                    Dictionary<byte[], string> blankHashes = new Dictionary<byte[], string>();
                    Dictionary<string, string[]> newScripts = new Dictionary<string, string[]>();
                    foreach (KeyValuePair<string, MintScript> pair in attackPowerScripts)
                    {
                        string[] script = pair.Value.WriteText(ref blankHashes);
                        for (int i = 0; i < script.Length; i++)
                        {
                            if (script[i].StartsWith("\t\t\tldsrc4 r0, "))
                            {
                                script[i] = "\t\t\tldsrc4 r0, " + rng.Next(1, limitDamage);
                            }
                        }
                        newScripts.Add(pair.Key, script);
                    }
                    foreach (KeyValuePair<string, string[]> pair in newScripts)
                    {
                        Console.WriteLine($"Processing {pair.Key}...");
                        mintStep.Scripts[pair.Key] = new MintScript(pair.Value, new byte[] { 2, 1, 5, 1 });
                    }
                }
                */
                string[] shuffledStoryStages = StoryStages.OrderBy(x => rng.Next()).ToArray();
                MintClass storyStageKind = mintStep.Scripts["Storage.File.StoryStageKind"].Classes[0];
                storyStageKind.Constants.Clear();
                for (int i = 0x0; i < shuffledStoryStages.Length; i++)
                {
                    storyStageKind.Constants.Add(new MintClass.MintConstant(shuffledStoryStages[i], (uint)i + 0x2));
                }
                storyStageKind.Constants.Add(new MintClass.MintConstant("Invalid", 0x0));
                storyStageKind.Constants.Add(new MintClass.MintConstant("Level0Stage1", 0x1));
                storyStageKind.Constants.Add(new MintClass.MintConstant("Level6Stage6", 0x20));
                storyStageKind.Constants.Add(new MintClass.MintConstant("Level7Stage1", 0x21));
                storyStageKind.Constants.Add(new MintClass.MintConstant("TERM", 0x22));
            }

            if (!Directory.Exists(outDir + "\\basil"))
                Directory.CreateDirectory(outDir + "\\basil");

            Console.WriteLine("Saving Seq Mint binary...");
            mintStep.Write(outDir + "\\basil\\Seq.bin");

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
