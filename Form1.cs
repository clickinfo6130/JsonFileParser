﻿//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace JsonFileParsing
{

    public partial class Form1 : Form
    {
        const int PART_SERIES_FILELOAD = 1;   // 1: 개별 파일 로드 하는것, 2 : 하나의 파일 사용
        public class Series
        {
            public string name { get; set; }
            public int id { get; set; }
            public List<string> ordercode { get; set; }
            public List<Option> option { get; set; } // array
        }

        public class Option
        {
            public int id { get; set; }
            public string name { get; set; }
            public string default_Value { get; set; }
            public List<Value> values { get; set; }  // array
            public string type { get; set; }
        }

        public class Value
        {
            public int enumid { get; set; }
            public string name { get; set; }
            public string desc { get; set; }
            public List<string> filter { get; set; } // array
            //  public List<string> FilterValues { get; set; }
            public List<List<List<string>>> filter_Values { get; set; }
        }

        public class Root
        {
            public List<Series> Series { get; set; }
        }
        Root root;

        //------------- write 클래스 ----------------//
        public class Seriesw
        {
            public string name { get; set; }
            public int id { get; set; }
            public List<string> ordercode { get; set; }                        
            public List<Position_with_length> position_with_length { get; set; } // array
            public List<int> rotation { get; set; }
            public List<int> camera_position { get; set; }
            public List<int> camera_center { get; set; }

            public List<Optionw> option { get; set; } // array
        }
        public class Position_with_length
        {
            public int index { get; set; }
            public List<string> solid { get; set; }
            public Position_with_length()
            {
                solid = new List<string>();
            }
        }
        public class Optionw
        {
            public int id { get; set; }
            public string name { get; set; }
            public string default_Value { get; set; }
            public List<Valuew> values { get; set; }  // array
            public string type { get; set; }
        }

        public class Valuew
        {
            public int enumid { get; set; }
            public string name { get; set; }
            public string desc { get; set; }
            public List<string> filter { get; set; } // array
            //  public List<string> FilterValues { get; set; }
            public List<List<List<string>>> filter_Values { get; set; }
            public string model_file_name { get; set; }
            public List<string> visible_on { get; set; }
            public List<string> visible_off { get; set; }
            public List<string> lengthValues { get; set; }
            public List<List<string>> rotationX { get; set; }
            public List<List<string>> rotationY { get; set; }
            public List<List<string>> rotationZ { get; set; }
        }
        //------------------------------------------//

        public class Rootw
        {
            public List<Seriesw> Series { get; set; }
        }
        Rootw rootw;
        Seriesw backSeries;
        string backSelectSeriesName;
        string backSelectOptionName;
        bool bValueTreeClickCheck;  // value tree를 클릭 했는지에 대한 정보 
        Optionw backSelectOption;
        string backSelectOptionValueName;

        public Form1()
        {
            InitializeComponent();
            LoadTextFileInfo(false);
        }

        private void LoadTextFileInfo(bool bwritechk)
        {
            string filepath = Application.StartupPath;

            if(bwritechk==false)
            {
                string filename = filepath + @"\setup.txt";
                if(File.Exists(filename))
                {
                    using (StreamReader readefile = new StreamReader(filename))
                    {
                        string line;
                        while ((line = readefile.ReadLine()) != null)
                        {
                            tbJsonFileFolder.Text = line;
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter outputfile = new StreamWriter(filepath + @"\setup.txt"))
                {
                    outputfile.WriteLine(tbJsonFileFolder.Text.Trim());
                }
            }
        }


    private void btn_JsonFile_Read_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.DefaultExt = "*.json";
            ofd.InitialDirectory = ".";
            ofd.Filter = "json filed (*.json) | *.json";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                string spath = ofd.FileName;
                tbFileName.Text = spath;
                string sfilename = ofd.SafeFileName.Substring(0, ofd.SafeFileName.Length - 4);
                button1_Click(null,null);
                button1_Click_1(null, null);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var optios = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string jsonString = string.Empty;
            if (PART_SERIES_FILELOAD == 1)
            {
                string sfilepath = tbJsonFileFolder.Text + "\\" + tbFileName.Text + ".json";
                if (File.Exists(sfilepath))
                    jsonString = File.ReadAllText(sfilepath);
                else
                    MessageBox.Show("파일이 없습니다.!! 파일 유무를 확인 해주세요");
            }
            else
            {
                jsonString = File.ReadAllText(tbFileName.Text);
            }            

            if(PART_SERIES_FILELOAD==2)
            {
                root = JsonSerializer.Deserialize<Root>(jsonString, optios);
                tvSeriesName.Nodes.Clear();

                foreach (var series in root.Series)
                {
                    tvSeriesName.Nodes.Add(series.name);
                }
            }
            else
            {
                rootw = JsonSerializer.Deserialize<Rootw>(jsonString, optios);
            }
        }

        private void btnWriteJson_Test_Click(object sender, EventArgs e)
        {
            string[] SORDER = new string[3];

            SORDER[0] = "1";
            SORDER[1] = "2";
            SORDER[2] = "3";

            int count = 0;
            foreach (var series1 in root.Series)
            {
                if (count > 2)
                    break;
                var root1 = new Root
                {
                    Series = new List<Series>
                    {
                        new Series
                        {
                            name = series1.name,
                            id = series1.id,
                            ordercode = new List<string>(series1.ordercode),                            
                            option = new List<Option>(series1.option),
                            //{
                            //    new Option
                            //    {
                            //        id = 1,
                            //        name = "Option1",
                            //        defaultValue = "Default",
                            //        values = new List<Value>
                            //        {
                            //            new Value
                            //            {
                            //                enumid = 1,
                            //                name = "Value1",
                            //                desc = "Description1",
                            //                filter = new List<string>
                            //                {
                            //                },
                            //                filterValues = new List<List<List<string>>>
                            //                {
                            //                }
                            //            }
                            //        },
                            //        type = "Type1"
                            //    }
                            //}
                        }
                    }
                };
                var optios = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,                    
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(root1, optios);
                string sfilename = "D:\\Extra\\c#\\JsonFile_Parser\\sampledata\\wdata" + (count + 1).ToString() + ".json";
                File.WriteAllText(sfilename, jsonString);
                count++;
            }
        }

        private void tvSeriesName_AfterSelect(object sender, TreeViewEventArgs e)
        {         

            ComponentDataInitial(true);
            string sSeriesName = "";
            tvOptionValue.Nodes.Clear();
            sSeriesName = e.Node.Text;
            LoadSelectSeriesFile(sSeriesName);
            foreach (var series1 in rootw.Series)
            {
                if(series1.name == sSeriesName)
                {
                    LoadSeriesOption(series1.option);
                    backSeries = series1;
                    backSelectSeriesName = sSeriesName;
                    LoadSeriesData(series1);
                    break;
                }
            }
            bValueTreeClickCheck = false;
        }

        private void LoadSelectSeriesFile(string seriesname)
        {
            tbFileName.Text = seriesname;
            button1_Click(null, null);
            //button1_Click_1(null, null);
        }

        private void LoadSeriesData(Seriesw series2)
        {
            int nidx = 0;

            if (series2.position_with_length == null)
                return;
            int ncnt = series2.position_with_length.Count;
            dgposwidthLength.Rows.Clear();

            int nindex = 0;
            if (ncnt>0)
            {
                int nColumncnt = dgposwidthLength.ColumnCount;
                string[] data1 = new string[nColumncnt];

                for (int i = 0; i < nColumncnt; i++)
                    data1[i] = string.Empty;

                for(int i=0;i<ncnt;i++)
                {
                    data1[0] = series2.position_with_length[i].index.ToString();
                    
                    int soilcnt = series2.position_with_length[i].solid.Count;
                    if (soilcnt > (nColumncnt-1)) soilcnt = nColumncnt-1;                  
                    for (int m=0;m<soilcnt;m++)
                    {                        
                        data1[m + 1] = series2.position_with_length[i].solid[m].ToString();
                    }

                    dgposwidthLength.Rows.Add(data1);
                }
            }
            if(series2.rotation.Count>0)
            {
                tbRotationX.Text = series2.rotation[0].ToString();
                tbRotationY.Text = series2.rotation[1].ToString();
                tbRotationZ.Text = series2.rotation[2].ToString();
            }

            if (series2.camera_center.Count > 0)
            {
                tbCamera_positionX.Text = series2.camera_center[0].ToString();
                tbCamera_positionY.Text = series2.camera_center[1].ToString();
                tbCamera_positionZ.Text = series2.camera_center[2].ToString();
            }

            if (series2.camera_position.Count > 0)
            {
                tbcamera_centerX.Text = series2.camera_position[0].ToString();
                tbcamera_centerY.Text = series2.camera_position[1].ToString();
                tbcamera_centerZ.Text = series2.camera_position[2].ToString();
            }

        }
        private void btnJsonFileWrite_Click(object sender, EventArgs e)
        {
            int count = 0;
            
            foreach (var series1 in root.Series)
            {
                var root1 = new Root
                {
                    Series = new List<Series>
                    {
                        new Series
                        {
                            name = series1.name,
                            id = series1.id,
                            ordercode = new List<string>(series1.ordercode),
                            option = new List<Option>(series1.option),
                        }
                    }
                };
                var optios = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(root1, optios);
                string sfilename = "D:\\Extra\\c#\\JsonFile_Parser\\sampledata\\개별파일\\" + series1.name + ".json";
                File.WriteAllText(sfilename, jsonString);
                count++;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int count = 0;
            rootw = new Rootw();

            rootw.Series = root.Series.Select(s => new Seriesw
            {
                name = s.name,
                id = s.id,
                ordercode = s.ordercode,
                position_with_length = new List<Position_with_length>(), // Initialize as needed
                rotation = new List<int>(), // Initialize as needed
                camera_position = new List<int>(), // Initialize as needed
                camera_center = new List<int>(), // Initialize as needed
                option = s.option.Select(o => new Optionw
                {
                    id = o.id,
                    name = o.name,
                    default_Value = o.default_Value,
                    values = o.values.Select(v => new Valuew
                    {
                        enumid = v.enumid,
                        name = v.name,
                        desc = v.desc,
                        filter = v.filter,
                        filter_Values = v.filter_Values,
                        model_file_name = "", // Initialize as needed
                        visible_on = new List<string>(), // Initialize as needed
                        visible_off = new List<string>(), // Initialize as needed
                        lengthValues = new List<string>(), // Initialize as needed
                        rotationX = new List<List<string>>(), // Initialize as needed
                        rotationY = new List<List<string>>(), // Initialize as needed
                        rotationZ = new List<List<string>>() // Initialize as needed
                    }).ToList(),
                    type = o.type,
                }).ToList()
            }).ToList();

        }
        private List<Optionw> GetUpdatedOptions(List<Optionw> option)
        {
            List<Optionw> Rtnoption = new List<Optionw>();
            Rtnoption = option;

            int ncnt = Rtnoption.Count;
            for(int i=0;i<ncnt;i++)
            {
               // Rtnoption[i].model_file_name = tbmodel_file_name.Text.Trim();
            }

            return Rtnoption;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int count = 0;
            List<Position_with_length> pwl = new List<Position_with_length>();
           
            int nrow = dgposwidthLength.RowCount;
            int ncol = dgposwidthLength.ColumnCount;
            string sdata = "";

            for (int i=0;i< nrow; i++)
            {
                if(dgposwidthLength.Rows[i].Cells[0].Value != null)
                {
                    Position_with_length  pl = new Position_with_length();
                    pl.index = int.Parse(dgposwidthLength.Rows[i].Cells[0].Value.ToString());
                    for (int j=1; j< ncol;j++)
                    {                       
                        if (dgposwidthLength.Rows[i].Cells[j].Value != null)
                        {
                            sdata = dgposwidthLength.Rows[i].Cells[j].Value.ToString().Trim();
                            pl.solid.Add(sdata);
                        }                          
                    }
                    pwl.Add(pl);                        
                }
            }
            List<int> rot = new List<int>();
            rot.Add(tbRotationX.Text.Trim() == "" ? 0 :int.Parse(tbRotationX.Text));
            rot.Add(tbRotationY.Text.Trim() == "" ? 0 : int.Parse(tbRotationY.Text));
            rot.Add(tbRotationZ.Text.Trim() == "" ? 0 : int.Parse(tbRotationZ.Text));

            List<int> campos = new List<int>();
            campos.Add(tbCamera_positionX.Text.Trim() == "" ? 0 : int.Parse(tbCamera_positionX.Text));
            campos.Add(tbCamera_positionY.Text.Trim() == "" ? 0 : int.Parse(tbCamera_positionY.Text));
            campos.Add(tbCamera_positionZ.Text.Trim() == "" ? 0 : int.Parse(tbCamera_positionZ.Text));

            List<int> camcen = new List<int>();
            camcen.Add(tbcamera_centerX.Text.Trim() == "" ? 0 : int.Parse(tbcamera_centerX.Text));
            camcen.Add(tbcamera_centerY.Text.Trim() == "" ? 0 : int.Parse(tbcamera_centerY.Text));
            camcen.Add(tbcamera_centerZ.Text.Trim() == "" ? 0 : int.Parse(tbcamera_centerZ.Text));

            

            foreach (var series1 in rootw.Series)
            {
              //  List<Optionw> updatedOptions = GetUpdatedOptions(series1.option);
              //  series1.option = updatedOptions;
                if (count > 2)
                    break;
                var root1 = new Rootw
                {
                    Series = new List<Seriesw>
                    {
                        new Seriesw
                        {
                            name = series1.name,
                            id = series1.id,
                            ordercode = new List<string>(series1.ordercode),
                            //position_with_length = series1.position_with_length,
                            //rotation = series1.rotation,
                            //camera_position = series1.camera_position,
                            //camera_center = series1.camera_center,                     
                            position_with_length = new List<Position_with_length>(pwl),
                            rotation = new List<int>(rot),
                            camera_position = new List<int>(campos),
                            camera_center = new List<int>(camcen),
                            option = new List<Optionw>(series1.option),
                        }
                    }
                };
                var optios = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(root1, optios);
                string sfilename = "D:\\Extra\\c#\\JsonFile_Parser\\sampledata\\" + series1.name + ".json";
                File.WriteAllText(sfilename, jsonString);
                count++;
                
            }
        }

        private void LoadSeriesOption(List<Optionw> option)
        {
            tvSeriesOption.Nodes.Clear();
            int ncnt = option.Count;
            for (int i = 0; i < ncnt; i++)
            {
                tvSeriesOption.Nodes.Add(option[i].name);
            }
        }
        private void tvSeriesOption_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ComponentDataInitial(false);
            string soptionName = "";
            soptionName = e.Node.Text;
            foreach (var option1 in backSeries.option)
            {
                if (option1.name == soptionName)
                {
                    LoadSeriesOptionValue(option1.values, false, "");
                    backSelectOptionName = option1.name;
                    break;
                }
            }
            bValueTreeClickCheck = false;
        }
        private void LoadSeriesOptionValue(List<Valuew> value,  bool bvaluechk, string svaluename)
        {
            int ncnt = value.Count;
            int nValueCnt = 0;
            string sitem = "";
            if (bvaluechk == false)
            {
                tvOptionValue.Nodes.Clear();
                for (int i = 0; i < ncnt; i++)
                {
                    sitem = value[i].name;
                    if (value[i].desc.Trim() != "")
                        sitem = sitem + " : " + value[i].desc;
                    tvOptionValue.Nodes.Add(sitem);
                }
            }
            else
            {
                for (int i = 0; i < ncnt; i++)
                {
                    if(svaluename.Trim() == value[i].name.Trim())
                    {
                        tbmodel_file_name.Text = value[i].model_file_name;

                        //-------vision on
                        dbgVision_on.Rows.Clear();
                        int ncnt1 = value[i].visible_on.Count;
                        if(ncnt1>0)
                        {
                            for (int n = 0; n < ncnt1; n++)
                                dbgVision_on.Rows.Add(value[i].visible_on[n]);
                        }

                        //-------vision ooff
                        dbgVision_off.Rows.Clear();
                        ncnt1 = value[i].visible_off.Count;
                        if (ncnt1 > 0)
                        {
                            for (int n = 0; n < ncnt1; n++)
                                dbgVision_off.Rows.Add(value[i].visible_off[n]);
                        }

                        //-------lengthValue
                        dbglengthValues.Rows.Clear();
                        ncnt1 = value[i].lengthValues.Count;
                        if (ncnt1 > 0)
                        {
                            for (int n = 0; n < ncnt1; n++)
                                dbglengthValues.Rows.Add(value[i].lengthValues[n]);
                        }

                        //-------rotation X
                        dbgrotationX.Rows.Clear();
                        ncnt1 = value[i].rotationX.Count;
                        if (ncnt1 > 0)
                        {
                            for (int n = 0; n < ncnt1; n++)
                            {
                                List<string> element = value[i].rotationX[n];
                                dbgrotationX.Rows.Add(element[0], element[1]);                                
                            }                                
                        }

                        //-------rotation Y
                        dbgrotationY.Rows.Clear();
                        ncnt1 = value[i].rotationY.Count;
                        if (ncnt1 > 0)
                        {
                            for (int n = 0; n < ncnt1; n++)
                            {
                                List<string> element = value[i].rotationY[n];
                                dbgrotationY.Rows.Add(element[0], element[1]);
                            }
                        }

                        //-------rotation Z
                        dbgrotationZ.Rows.Clear();
                        ncnt1 = value[i].rotationZ.Count;
                        if (ncnt1 > 0)
                        {
                            for (int n = 0; n < ncnt1; n++)
                            {
                                List<string> element = value[i].rotationZ[n];
                                dbgrotationZ.Rows.Add(element[0], element[1]);
                            }
                        }

                        break;
                    }
                }
            }

        }

        private void tvOptionValue_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ComponentDataInitial(false);
            string sValueName = "";
            sValueName = e.Node.Text.Trim();
            if (sValueName.Trim() == "")
                return;
            foreach (var option1 in backSeries.option)
            {
                if (option1.name == backSelectOptionName)
                {
                    int iFind = sValueName.IndexOf(':');
                    string sTemp = "";
                    sTemp = sValueName.Substring(0, iFind);
                    if(sTemp.Trim() != "")
                        LoadSeriesOptionValue(option1.values, true, sTemp);
                    backSelectOption = option1;
                    backSelectOptionValueName = sTemp;
                    break;
                }
            }
            bValueTreeClickCheck = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<List<string>> list = new List<List<string>>();
            //{
            //    new List<string> {"a", "b", "c"},
            //    new List<string> {"d", "e", "f"},
            //    new List<string> {"g", "h", "i"},
            //};

            List<string> sdata = new List<string>();
            sdata.Add("a");
            sdata.Add("b");
            list.Add(sdata);
            // List<string> sdata1 = new List<string>();
            sdata = new List<string>();
            sdata.Add("f");
            sdata.Add("g");
            list.Add(sdata);

            string sm = "";
            dbgrotationX.Rows.Clear();
            for (int i = 0; i < list.Count; i++) {
                List<string> element = list.ElementAt(i);
                int mm = element.Count;
                sm = string.Join(",", element);
                dbgrotationX.Rows.Add(element[0], element[1]);
                //Console.WriteLine(string.Join(", ", element));
            }
        }

        private void btnSeriesDataSave_Click(object sender, EventArgs e)
        {
            if(PART_SERIES_FILELOAD == 2)
            {
                int nidx = 0;
                foreach (var series2 in rootw.Series)
                {
                    if (series2.name.Trim() == backSelectSeriesName.Trim())
                    {
                        List<Position_with_length> pwl = new List<Position_with_length>();

                        int nrow = dgposwidthLength.RowCount;
                        int ncol = dgposwidthLength.ColumnCount;
                        string sdata = "";

                        for (int i = 0; i < nrow; i++)
                        {
                            if (dgposwidthLength.Rows[i].Cells[0].Value != null)
                            {
                                Position_with_length pl = new Position_with_length();
                                pl.index = int.Parse(dgposwidthLength.Rows[i].Cells[0].Value.ToString());
                                for (int j = 1; j < ncol; j++)
                                {
                                    if (dgposwidthLength.Rows[i].Cells[j].Value != null)
                                    {
                                        sdata = dgposwidthLength.Rows[i].Cells[j].Value.ToString().Trim();
                                        pl.solid.Add(sdata);
                                    }
                                }
                                pwl.Add(pl);
                            }
                        }
                        List<int> rot = new List<int>();
                        rot.Add(tbRotationX.Text.Trim() == "" ? 0 : int.Parse(tbRotationX.Text));
                        rot.Add(tbRotationY.Text.Trim() == "" ? 0 : int.Parse(tbRotationY.Text));
                        rot.Add(tbRotationZ.Text.Trim() == "" ? 0 : int.Parse(tbRotationZ.Text));

                        List<int> campos = new List<int>();
                        campos.Add(tbCamera_positionX.Text.Trim() == "" ? 0 : int.Parse(tbCamera_positionX.Text));
                        campos.Add(tbCamera_positionY.Text.Trim() == "" ? 0 : int.Parse(tbCamera_positionY.Text));
                        campos.Add(tbCamera_positionZ.Text.Trim() == "" ? 0 : int.Parse(tbCamera_positionZ.Text));

                        List<int> camcen = new List<int>();
                        camcen.Add(tbcamera_centerX.Text.Trim() == "" ? 0 : int.Parse(tbcamera_centerX.Text));
                        camcen.Add(tbcamera_centerY.Text.Trim() == "" ? 0 : int.Parse(tbcamera_centerY.Text));
                        camcen.Add(tbcamera_centerZ.Text.Trim() == "" ? 0 : int.Parse(tbcamera_centerZ.Text));

                        //series2.position_with_length = pwl;
                        //series2.rotation = rot;
                        //series2.camera_position = campos;
                        //series2.camera_center = camcen;

                        rootw.Series[nidx].position_with_length = pwl;
                        rootw.Series[nidx].rotation = rot;
                        rootw.Series[nidx].camera_position = campos;
                        rootw.Series[nidx].camera_center = camcen;
                        break;
                    }
                    nidx++;
                }
            }
            else
            {
                button2_Click(null, null);
            }


        }
        private void btnValueDataSave_Click(object sender, EventArgs e)
        {
            string sValueName = "";
   
            foreach (var value1 in backSelectOption.values)
            {
                if (value1.name.Trim() == backSelectOptionValueName.Trim())
                {
                    value1.model_file_name = tbmodel_file_name.Text.Trim();

                    //-------vision on
                    int ncnt1 = dbgVision_on.Rows.Count;
                    if (ncnt1 > 0)
                    {
                        value1.visible_on.Clear();
                        for (int n = 0; n < ncnt1; n++)
                        {
                            if (dbgVision_on.Rows[n].Cells[0].Value != null)
                                value1.visible_on.Add(dbgVision_on.Rows[n].Cells[0].Value.ToString());
                        }                            
                    }

                    //-------vision ooff                    
                    ncnt1 = dbgVision_off.Rows.Count;
                    if (ncnt1 > 0)
                    {
                        value1.visible_off.Clear();
                        for (int n = 0; n < ncnt1; n++)
                        {
                            if (dbgVision_off.Rows[n].Cells[0].Value != null)
                                value1.visible_off.Add(dbgVision_off.Rows[n].Cells[0].Value.ToString());
                        }
                    }

                    //-------lengthValue
                    ncnt1 = dbglengthValues.Rows.Count;
                    if (ncnt1 > 0)
                    {
                        value1.lengthValues.Clear();
                        for (int n = 0; n < ncnt1; n++)
                        {
                            if (dbglengthValues.Rows[n].Cells[0].Value != null)
                                value1.lengthValues.Add(dbglengthValues.Rows[n].Cells[0].Value.ToString());
                        }
                    }
                    //-------rotation X
                    ncnt1 = dbgrotationX.Rows.Count;
                    if (ncnt1 > 0)
                    {
                        value1.rotationX.Clear();
                        for (int n = 0; n < ncnt1; n++)
                        {
                            if (dbgrotationX.Rows[n].Cells[0].Value != null && dbgrotationX.Rows[n].Cells[1].Value != null)
                            {
                                List<string> element = new List<string> { dbgrotationX.Rows[n].Cells[0].Value.ToString(), dbgrotationX.Rows[n].Cells[1].Value.ToString() };
                                value1.rotationX.Add(element);                             
                            }
                        }
                    }

                    ////-------rotation Y
                    ///
                    ncnt1 = dbgrotationY.Rows.Count;
                    if (ncnt1 > 0)
                    {
                        value1.rotationY.Clear();
                        for (int n = 0; n < ncnt1; n++)
                        {
                            if (dbgrotationY.Rows[n].Cells[0].Value != null && dbgrotationY.Rows[n].Cells[1].Value != null)
                            {
                                List<string> element = new List<string> { dbgrotationY.Rows[n].Cells[0].Value.ToString(), dbgrotationY.Rows[n].Cells[1].Value.ToString() };
                                value1.rotationY.Add(element);
                            }
                        }
                    }

                    ////-------rotation Z
                    ///
                    ncnt1 = dbgrotationZ.Rows.Count;
                    if (ncnt1 > 0)
                    {
                        value1.rotationZ.Clear();
                        for (int n = 0; n < ncnt1; n++)
                        {
                            if (dbgrotationZ.Rows[n].Cells[0].Value != null && dbgrotationZ.Rows[n].Cells[1].Value != null)
                            {
                                List<string> element = new List<string> { dbgrotationZ.Rows[n].Cells[0].Value.ToString(), dbgrotationZ.Rows[n].Cells[1].Value.ToString() };
                                value1.rotationZ.Add(element);
                            }
                        }
                    }

                    break;
                }
            }
        }

        private void  ComponentDataInitial(bool btype)
        {
            if(btype == true)
            {
                // 시리즈 데이터
                dgposwidthLength.Rows.Clear();
                tbRotationX.Text = string.Empty;
                tbRotationY.Text = string.Empty;
                tbRotationZ.Text = string.Empty;
                tbCamera_positionX.Text = string.Empty;
                tbCamera_positionY.Text = string.Empty;
                tbCamera_positionZ.Text = string.Empty;
                tbcamera_centerX.Text = string.Empty;
                tbcamera_centerY.Text = string.Empty;
                tbcamera_centerZ.Text = string.Empty;
            }
            //옵션 Value 데이터
            tbmodel_file_name.Text = string.Empty;
            dbgVision_on.Rows.Clear();
            dbgVision_off.Rows.Clear();
            dbglengthValues.Rows.Clear();
            dbgrotationX.Rows.Clear();
            dbgrotationY.Rows.Clear();
            dbgrotationZ.Rows.Clear();
        }

        private void TreeView_Leave(object sender, EventArgs e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView != null && treeView.SelectedNode != null)
            {
                treeView.SelectedNode.BackColor = SystemColors.Highlight;
                treeView.SelectedNode.ForeColor = Color.White;
            }
        }

        private void TreeView_Enter(object sender, EventArgs e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView != null && treeView.SelectedNode != null)
            {
                treeView.SelectedNode.BackColor = Color.Empty;
                treeView.SelectedNode.ForeColor = Color.Empty;
            }
        }

        private void btnjsonfileFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tbJsonFileFolder.Text = folderBrowserDialog.SelectedPath;
                LoadTextFileInfo(true);
            }
        }

        private void btnSeriesNameSave_Click(object sender, EventArgs e)
        {
            string filepath = "D:\\Extra\\c#\\JsonFile_Parser\\sampledata\\SeriesName.txt"; //Application.StartupPath;
            using (StreamWriter outputfile = new StreamWriter(filepath)) // +@"\SeriesName.txt"))
            {
                int icount = tvSeriesName.Nodes.Count;
                for (int i = 0; i < icount; i++)
                {
                    outputfile.WriteLine(tvSeriesName.Nodes[i].Text);
                }
            }
        }

        private void btnSeriesLoad_Click(object sender, EventArgs e)
        {
            string filepath = Application.StartupPath;
            string filename = filepath + @"\SeriesName.txt";
            if (File.Exists(filename))
            {
                using (StreamReader readefile = new StreamReader(filename))
                {
                    tvSeriesName.Nodes.Clear();
                    string line;
                    while ((line = readefile.ReadLine()) != null)
                    {
                        tvSeriesName.Nodes.Add(line);
                    }
                }
            }
        }
    }
}