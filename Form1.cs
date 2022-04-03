using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpentkMouseTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            glControl.Load += glControl_Load;
            glControl.Resize += glControl_Resize;
            glControl.MouseDown += glControl_MouseDown; ;
            glControl.MouseMove += glControl_MouseMove; ;
            glControl.MouseWheel += glControl_MouseWheel;
            glControl.MouseUp += glControl_MouseUp;
            glControl.Paint += glControl_Paint; ;
        }

        float[,] ZairyoXYZ = new float[2, 3] { { 0, 0, 0 }, { -100, -70, -20 } };

        private void DrawBox()
        {
            float x1 = ZairyoXYZ[0, 0];
            float y1 = ZairyoXYZ[0, 1];
            float z1 = ZairyoXYZ[0, 2];
            float x2 = ZairyoXYZ[1, 0];
            float y2 = ZairyoXYZ[1, 1];
            float z2 = ZairyoXYZ[1, 2];

            GL.Color3(Color.Yellow);
            GL.Begin(BeginMode.LineLoop);
            {
                GL.Vertex3(x1, y1, z1);
                GL.Vertex3(x1, y2, z1);
                GL.Vertex3(x2, y2, z1);
                GL.Vertex3(x2, y1, z1);
            }
            GL.End();
            GL.Begin(BeginMode.LineLoop);
            {
                GL.Vertex3(x1, y1, z2);
                GL.Vertex3(x1, y2, z2);
                GL.Vertex3(x2, y2, z2);
                GL.Vertex3(x2, y1, z2);
            }
            GL.End();
            GL.Begin(BeginMode.Lines);
            {
                GL.Vertex3(x1, y1, z1);
                GL.Vertex3(x1, y1, z2);
                GL.Vertex3(x2, y2, z1);
                GL.Vertex3(x2, y2, z2);
                GL.Vertex3(x1, y2, z1);
                GL.Vertex3(x1, y2, z2);
                GL.Vertex3(x2, y1, z1);
                GL.Vertex3(x2, y1, z2);
            }
            GL.End();
        }

        private void SetProjection()
        {
            // ビューポートの設定
            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            // 視体積の設定
            GL.MatrixMode(MatrixMode.Projection);

            Matrix4 proj = Matrix4.CreateOrthographicOffCenter(-(float)glControl.Width / 2, (float)glControl.Width / 2, -(float)glControl.Height / 2, (float)glControl.Height / 2, -1000, 1000);

            GL.LoadMatrix(ref proj);

            // MatrixMode を元に戻す
            GL.MatrixMode(MatrixMode.Modelview);
        }

        float bigsmall = 1f;
        float InitBigsmall = 3f; // 初期状態の拡大率

        private void glControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(glControl.BackColor);

            // Projection の設定
            SetProjection();

            // デプスバッファの使用
            GL.Enable(EnableCap.DepthTest);

            // 視界の設定
            SetInitSight();
            SetInitScale();
        }

        void SetInitSight()
        {
            EyeZ = 1f;
            SetSight();
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            SetProjection();

            // 再描画
            glControl.Refresh();
        }

        //

        // 立方体のX座標
        float x0 = 0;
        float X
        {
            get { return x0; }
            set { x0 = (float)Math.Round(value, 2, MidpointRounding.AwayFromZero); }
        }

        // 立方体のY座標
        float y0 = 0;
        float Y
        {
            get { return y0; }
            set { y0 = (float)Math.Round(value, 2, MidpointRounding.AwayFromZero); }
        }

        // 立方体のZ座標
        float z0 = 0;
        float Z
        {
            get { return z0; }
            set { z0 = (float)Math.Round(value, 2, MidpointRounding.AwayFromZero); }
        }

        // X軸を中心に回転させる角度
        float rx0 = 0;
        float RotateX
        {
            get { return rx0; }
            set
            {
                if (value < 0)
                    value += 360;
                rx0 = value % 360;
            }
        }

        // Y軸を中心に回転させる角度
        float ry0 = 0;
        float RotateY
        {
            get { return ry0; }
            set
            {
                if (value < 0)
                    value += 360;
                ry0 = value % 360;
            }
        }

        // マウス座標（マウスをクリックした位置の保持用）
        PointF _oldPoint;
        PointF _realPoint;

        // 回転処理をする軸のX座標とY座標
        float AxisRotationX = 0;
        float AxisRotationY = 0;

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // glControl上におけるマウスの座標を取得する
                Point pt = glControl.PointToClient(Control.MousePosition);

                // ここから回転処理をする軸のX座標とY座標を取得する
                AxisRotationX = (pt.X - glControl.Width / 2) - X;
                AxisRotationY = -(pt.Y - glControl.Height / 2) - Y;
            }
            else if (e.Button == MouseButtons.Left)
            {
                _realPoint.X = X;
                _realPoint.Y = Y;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                ViewSettingInit(); // 中央のボタンがおされたらもとの状態に戻す
                glControl.Refresh();
            }

            // マウスをクリックした位置の記録
            _oldPoint.X = e.X;
            _oldPoint.Y = e.Y;
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 直方体を位置を変更する
                X = _realPoint.X + (e.X - _oldPoint.X);
                Y = _realPoint.Y - (e.Y - _oldPoint.Y);
                glControl.Refresh();
            }
            else if (e.Button == MouseButtons.Right)
            {
                // 直方体を回転させる
                RotateX = 180.0f * ((float)(e.Y - _oldPoint.Y) / glControl.Width);
                RotateY = 180.0f * ((float)(e.X - _oldPoint.X) / glControl.Height);
                glControl.Refresh();
            }
        }

        List<RotateScaleInfo> RotateScaleInfos = new List<RotateScaleInfo>();

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RotateX = 180.0f * ((float)(e.Y - _oldPoint.Y) / glControl.Width);
                RotateY = 180.0f * ((float)(e.X - _oldPoint.X) / glControl.Height);

                // 回転の処理がおこなわれていないなら保存の必要はない
                if (RotateX == 0 && RotateY == 0)
                    return;

                RotateScaleInfo info = new RotateScaleInfo();
                info.RotateInfo = new RotateInfo(
                    AxisRotationX,
                    AxisRotationY,
                    RotateX,
                    RotateY
                );
                RotateScaleInfos.Insert(0, info);

                AxisRotationX = 0;
                AxisRotationY = 0;
                RotateX = 0;
                RotateY = 0;
            }
        }

        // 最後に取得されたデータ
        float LastScaleCenterX = 0;
        float LastScaleCenterY = 0;
        float LastBigsmall = 0;

        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                bigsmall += 0.02f;
            else
            {
                bigsmall -= 0.02f;
                if (bigsmall < 0.01)
                    bigsmall = 0.01f;
            }

            Point pt = glControl.PointToClient(Control.MousePosition);

            // 拡大縮小の中心点を求める
            float scaleCenterX = (pt.X - glControl.Width / 2) - X;
            float scaleCenterY = -(pt.Y - glControl.Height / 2) - Y;

            // 拡大縮小率や拡大縮小の中心の座標が変化した場合はリストに格納
            if (LastScaleCenterX != scaleCenterX || LastScaleCenterY != scaleCenterY || LastBigsmall != bigsmall)
            {
                RotateScaleInfo info = new RotateScaleInfo();
                info.ScaleInfo = new ScaleInfo(scaleCenterX, scaleCenterY, (LastBigsmall == 0) ? bigsmall : bigsmall / LastBigsmall);
                RotateScaleInfos.Insert(0, info);
            }

            // 最後に取得されたデータは保存しておく
            LastScaleCenterX = scaleCenterX;
            LastScaleCenterY = scaleCenterY;
            LastBigsmall = bigsmall;

            glControl.Refresh();
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // 視点を変更する
            SetSight();

            DrawObject();
            glControl.SwapBuffers();
        }

        private void DrawObject()
        {
            GL.PushMatrix();
            {
                GL.Translate(X, Y, Z);

                // マウスドラッグによる回転処理がおこなわれていないときは
                // RotateXとRotateYは0なので何もおきない
                GL.Translate(AxisRotationX, AxisRotationY, 0);
                GL.Rotate(RotateY, 0, 1, 0);
                GL.Rotate(RotateX, 1, 0, 0);
                GL.Translate(-AxisRotationX, -AxisRotationY, 0);

                foreach (RotateScaleInfo rotateScaleInfo in RotateScaleInfos)
                {
                    if (rotateScaleInfo.RotateInfo != null)
                    {
                        RotateInfo info = rotateScaleInfo.RotateInfo;
                        GL.Translate(info.AxisRotationX, info.AxisRotationY, 0);
                        GL.Rotate(info.RotateY, 0, 1, 0);
                        GL.Rotate(info.RotateX, 1, 0, 0);
                        GL.Translate(-info.AxisRotationX, -info.AxisRotationY, 0);
                    }
                    if (rotateScaleInfo.ScaleInfo != null)
                    {
                        ScaleInfo info = rotateScaleInfo.ScaleInfo;
                        Scale(info.Scale, info.ScaleCenterX, info.ScaleCenterY);
                    }
                }

                DrawLine1();
                DrawBox();
                DrawLine2();
            }
            GL.PopMatrix();
        }

        void Scale(float scale, float centerX, float centerY)
        {
            GL.Translate(centerX, centerY, 0);
            GL.Scale(scale, scale, scale);
            GL.Translate(-centerX, -centerY, 0);
        }

        private void DrawLine1()
        {
            GL.Color3(Color.Gray);
            GL.Begin(BeginMode.Lines);
            {
                for (float i = -100; i <= 100; i = i + 10)
                {
                    GL.Vertex3(-100, i, -0.01f);
                    GL.Vertex3(100, i, -0.01f);
                }
                for (float i = -100; i <= 100; i = i + 10)
                {
                    GL.Vertex3(i, 100, -0.01f);
                    GL.Vertex3(i, -100, -0.01f);
                }
            }
            GL.End();

            GL.Color3(Color.Red);
            GL.LineWidth(3); // XYZ軸はやや太い直線にして先端に矢印をつける
            GL.Begin(BeginMode.Lines);
            {
                GL.Color3(Color.Red);
                GL.Vertex3(0, 0, 0.01f);
                GL.Vertex3(30, 0, 0.01f);
                GL.Vertex3(30, 0, 0.01f);
                GL.Vertex3(27, 3, 0.01f);
                GL.Vertex3(30, 0, 0.01f);
                GL.Vertex3(27, -3, 0.01f);

                GL.Color3(Color.Blue);
                GL.Vertex3(0, 0, 0.01f);
                GL.Vertex3(0, 30, 0.01f);
                GL.Vertex3(0, 30, 0.01f);
                GL.Vertex3(3, 27, 0.01f);
                GL.Vertex3(0, 30, 0.01f);
                GL.Vertex3(-3, 27, 0.01f);

                GL.Color3(Color.Green);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, 30);
                GL.Vertex3(3, 0, 27);
                GL.Vertex3(0, 0, 30);
                GL.Vertex3(0, 0, 30);
                GL.Vertex3(-3, 0, 27);
            }
            GL.End();
            GL.LineWidth(1);
        }

        private void DrawLine2()
        {
            GL.Color3(Color.Pink);
            GL.Begin(BeginMode.Lines);
            {
                GL.Vertex3(-5, -5, 0);
                GL.Vertex3(-5, -5, 10);
            }
            GL.End();

            Circle2D(2, -5, -5, 0);
        }

        void Circle2D(double radius, double x, double y, double z)
        {
            for (double th1 = 0.0; th1 <= 360.0; th1 = th1 + 1.0)
            {
                var th2 = th1 + 10.0;
                var th1_rad = th1 / 180.0 * Math.PI;
                var th2_rad = th2 / 180.0 * Math.PI;

                var x1 = radius * Math.Cos(th1_rad);
                var y1 = radius * Math.Sin(th1_rad);
                var x2 = radius * Math.Cos(th2_rad);
                var y2 = radius * Math.Sin(th2_rad);

                GL.Begin(BeginMode.LineLoop);
                {
                    GL.Color3(Color.Red);
                    GL.Vertex3(x1 + x, y1 + y, z);
                    GL.Vertex3(x2 + x, y2 + y, z);
                }
                GL.End();
            }
        }

        void ViewSettingInit()
        {
            X = 0f;
            Y = 0f;
            Z = 0f;
            RotateX = 0f;
            RotateY = 0f;
            EyeZ = 1f;

            this.RotateScaleInfos.Clear();
            LastBigsmall = 0;
            SetInitScale();
        }
        //

        // 視点のZ座標
        float eyeZ0 = 0;
        float EyeZ
        {
            get { return eyeZ0; }
            set { eyeZ0 = (float)Math.Round(value, 2, MidpointRounding.AwayFromZero); }
        }

        void SetInitScale()
        {
            bigsmall = InitBigsmall;

            // 原点を中心にInitBigsmall倍に拡大（縮小）する
            float centerX = 0;
            float centerY = 0;
            RotateScaleInfo info = new RotateScaleInfo();
            info.ScaleInfo = new ScaleInfo(centerX, centerY, (LastBigsmall == 0) ? bigsmall : bigsmall / LastBigsmall);
            RotateScaleInfos.Insert(0, info);

            LastScaleCenterX = centerX;
            LastScaleCenterY = centerY;
            LastBigsmall = bigsmall;

            glControl.Refresh();
        }

        void SetSight()
        {
            // 視界の設定
            Vector3 eye = new Vector3(0, 0, EyeZ);
            Vector3 target = new Vector3(0, 0, 0);
            Vector3 up = new Vector3(0, 1, 0);

            Matrix4 look = Matrix4.LookAt(eye, target, up);
            GL.LoadMatrix(ref look);
        }






    }

    public class GLControlEx : GLControl
    {
    }

    public class RotateScaleInfo
    {
        public RotateInfo RotateInfo = null;
        public ScaleInfo ScaleInfo = null;
    }

    public class RotateInfo
    {
        public RotateInfo(float axisRotationX, float axisRotationY, float rotateX, float rotateY)
        {
            AxisRotationX = axisRotationX;
            AxisRotationY = axisRotationY;
            RotateX = rotateX;
            RotateY = rotateY;
        }
        public float AxisRotationX = 0;
        public float AxisRotationY = 0;
        public float RotateX = 0;
        public float RotateY = 0;
    }

    public class ScaleInfo
    {
        public ScaleInfo(float scaleCenterX, float scaleCenterY, float scale)
        {
            ScaleCenterX = scaleCenterX;
            ScaleCenterY = scaleCenterY;
            Scale = scale;
        }
        public float ScaleCenterX = 0;
        public float ScaleCenterY = 0;
        public float Scale = 0;
    }


}
