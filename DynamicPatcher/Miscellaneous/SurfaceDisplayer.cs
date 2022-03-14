using DynamicPatcher;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Drawing;
// using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miscellaneous
{
#if false
    class DisplayerForm : Form
    {
        public DisplayerForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
        }
    }
    [RunClassConstructorFirst]
    public class SurfaceDisplayer
    {
        static SurfaceDisplayer()
        {
            new SurfaceDisplayer();
        }

        public SurfaceDisplayer()
        {
            Task.Run(CreateSurfaceWindow);
        }

        private void CreateSurfaceWindow()
        {
            Logger.LogWarning("Surface Displayer is unstable now.");
            Form form = new DisplayerForm();
            form.Size = new Size(680, 480);
            form.Text = "Surface Displayer";
            form.Paint += Form_Paint;

            form.ShowDialog();
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                var pSurface = Surface.Composite;
                Form form = sender as Form;

                if (pSurface.IsNull == false)
                {
                    var g = e.Graphics;

                    SetImageBuffer(ref pSurface.Ref);

                    g.DrawImage(_image, form.DisplayRectangle);
                }

                Task.Run(async () => await Task.Delay(TimeSpan.FromSeconds(1.0 / 60))).ContinueWith(_ => form.Refresh());
            }
            catch (Exception ex)
            {
                Logger.PrintException(ex);
            }
        }

        private void SetImageBuffer(ref Surface surface)
        {
            const PixelFormat fmt = PixelFormat.Format16bppRgb565;

            var rect = surface.GetRect();
            var size = new Size(rect.Width, rect.Height);

            if (_image == null || _image.Size != size)
            {
                _image?.Dispose();

                _image = new Bitmap(size.Width, size.Height, fmt);
            }

            var map = _image.LockBits(new Rectangle(new Point(0, 0), size), ImageLockMode.WriteOnly, fmt);
            Pointer<byte> dst = map.Scan0;
            Pointer<byte> src = surface.Lock(0, 0);

            if(!dst.IsNull && !src.IsNull)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    Helpers.Copy(src, dst, rect.Width * 2);
                    dst += map.Stride;
                    src += surface.GetPitch();
                }
            }

            surface.Unlock();
            _image.UnlockBits(map);
        }

        private Bitmap _image;
    }
#endif
}
