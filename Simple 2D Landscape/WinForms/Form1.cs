﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;
using Simple_2D_Landscape.LandscapeEngine;

namespace Simple_2D_Landscape
{
	public partial class Form1 : Form
	{
        private daseffect _daseffect;

        private Timer _timer;

        private void SetPicture(Bitmap input, bool Scale = true)
        {
            int Width = pictureBox1.Width;
            int Height = pictureBox1.Height;

            Bitmap image = input;

            if(Scale && (input.Width != Width || input.Height != Height))
            {
                image = new Bitmap(Width, Height);

                using(Graphics gr = Graphics.FromImage(image))
                {
                    gr.SmoothingMode = SmoothingMode.HighSpeed;
                    gr.CompositingQuality = CompositingQuality.HighSpeed;
                    gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                    gr.DrawImage(input, new Rectangle(0, 0, Width, Height));
                }
            }

            pictureBox1.Image = image;
        }

        private void InitializePhysicalModel()
        {
            _daseffect = new daseffect(256, 256, colorInterpretator: ColorInterpretationType.WaterFlow);
            
            _daseffect.AddNoise(0.0001f, 0.001f, 0.01f);
            _daseffect.AddNoise(0.5f, 1.0f, 0.001f);
             
            _daseffect.Set(0, _daseffect.Width >> 1, _daseffect.Height >> 1, 2.0f);
            _daseffect.Set(1, _daseffect.Width >> 1, _daseffect.Height >> 1, 2.0f);

            _daseffect.CurrentColorInterpretator = ColorInterpretationType.WaterFlow;

            SetPicture(_daseffect.GetBitmap());
        }

        private void InitializeTimer()
        {
            _timer = new Timer();

            _timer.Interval = 25;
            _timer.Enabled = false;

            _timer.Tick += new EventHandler(CalcTimerProcessor);
        }

        private void InitializeViewModel()
        {
            trackBar1.ValueChanged += TrackBar1_ValueChanged;

            trackBar1.Value = (int)(Math.Round(trackBar1.Maximum*_daseffect.WaterLevel / daseffect.MaxWaterLevel));

            comboBox1.DataSource =  Enum.GetValues(typeof(ColorInterpretationType));

            comboBox1.SelectedItem = _daseffect.CurrentColorInterpretator;

            comboBox1.SelectedValueChanged += ComboBox1_SelectedValueChanged;
        }

        private void ComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            _daseffect.CurrentColorInterpretator = (ColorInterpretationType)comboBox1.SelectedItem;

            SetPicture(_daseffect.GetBitmap());
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            _daseffect.WaterLevel = (float)trackBar1.Value/trackBar1.Maximum;
            
            label2.Text = Math.Round(100*_daseffect.WaterLevel) + "%";

            SetPicture(_daseffect.GetBitmap());
        }

        public Form1()
		{
			InitializeComponent();
            InitializePhysicalModel();
            InitializeViewModel();
            InitializeTimer();            
		}

        private void CalcTimerProcessor(Object myObject, EventArgs myEventArgs)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            _daseffect.IterationOptimazed();
            SetPicture(_daseffect.GetBitmap());
            sw.Stop();
            
            long calcTime = sw.ElapsedMilliseconds;
        }

        //private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        //{
        //    // Create a Simple Sample:

        //    OpenGL gl = openGLControl1.OpenGL;
            
        //    // Clear Screen & Depth Buffer
        //    gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            
        //    // Reset World Matrix
        //    gl.LoadIdentity();
            
        //    // Move Draw Pointer to -Z coords.
        //    gl.Translate(0.0f, 0.0f, -5.0f);
            
        //    // Begin Drawing
        //    gl.Begin(OpenGL.GL_TRIANGLES);
            
        //    // Use White Color
        //    gl.Color(1.0f, 1.0f, 1.0f);
            
        //    gl.Vertex(-1.0f, -1.0f);
        //    gl.Vertex(0.0f, 1.0f);
        //    gl.Vertex(1.0f, -1.0f);
            
        //    // Draw triangle with points(vertex) { { -1.0f, -1.0f }, { 0.0f, 1.0f }, { 1.0f, -1.0f } }

        //    // Stop Drawing
        //    gl.End();
        //}

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _timer.Enabled = !_timer.Enabled;
        }
    }
}
