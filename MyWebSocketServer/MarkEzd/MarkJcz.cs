using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace JczMark
{
    public class MarkJcz : LoadJczDll
    {
        public static bool mIsInitLaser { get; private set; }
        public static string mMarkEzdFile;
        public static LmcErrCode mLastError;
        private static object objectLock = new object();

        public static bool mEnableAxis0;
        public static bool mEnableAxis1;
        public static bool mHomeFlagAxis0;
        public static bool mHomeFlagAxis1;
        public static bool mIsJogMoving;

        private static bool mEnableRed;
        public static Thread mThreadRedLight;
        public static Thread mThreadInitLaser;

        #region 设备

        /// <summary>
        /// 激光器初始化
        /// 注意本地测试时bTestMode设置为true.到了现场,放到激光打印设备自带的电脑上,这里必须设置为false(或者不设置,保持默认的false,否则会报LCM硬件版本错误)
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="bTestMode"></param>
        /// <returns></returns>
        public static bool InitLaser(IntPtr hwnd, bool bTestMode = false)
        {
            mIsInitLaser = false;
            var strEzcadPath = Application.StartupPath + "\\";
            Console.WriteLine(strEzcadPath);
            if (!File.Exists(strEzcadPath + "MarkEzd.dll"))
            {
                mLastError = LmcErrCode.LMC1_ERR_NOFINDEZD;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_INITIAL(strEzcadPath, bTestMode, hwnd);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
            {
                mIsInitLaser = true;
                return true;
            }
            else
            {
                mLastError = ret;
                return false;
            }
        }

        /// <summary>
        /// 初始化激光器
        /// </summary>
        /// <returns></returns>
        public static bool InitLaser()
        {
            mIsInitLaser = false;
            var strEzcadPath = Application.StartupPath + "\\";
            if (!File.Exists(strEzcadPath + "MarkEzd.dll"))
            {
                mLastError = LmcErrCode.LMC1_ERR_NOFINDEZD;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_INITIAL2(strEzcadPath, false);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
            {
                mIsInitLaser = true;
                return true;
            }
            else
            {
                mLastError = ret;
                return false;
            }
        }

        /// <summary>
        /// 关闭激光器
        /// </summary>
        public static void Close()
        {
            if (mIsInitLaser)
            {
                LmcErrCode ret;
                lock (objectLock)
                {
                    ret = LMC1_CLOSE();
                }
                if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                {
                    mIsInitLaser = false;
                    mEnableAxis0 = false;
                    mEnableAxis1 = false;
                }
                else
                {
                    mLastError = ret;
                }
            }
        }

        /// <summary>
        /// 显示设备(F3)参数设置对话框   这个对话框尽量不要调用显示,要不会修改参数
        /// </summary>
        /// <returns></returns>
        public static bool ShowDevCfgForm()
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_SETDEVCFG();
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 设置旋转变换参数
        /// </summary>
        /// <param name="dMoveX"></param>
        /// <param name="dMoveY"></param>
        /// <param name="dCenX"></param>
        /// <param name="dCenY"></param>
        /// <param name="dRotateAng"></param>
        /// <param name="dScaleX"></param>
        /// <param name="dScaleY"></param>
        /// <returns></returns>
        public static bool SetRotateMoveParam(double dMoveX, double dMoveY, double dCenX, double dCenY, double dRotateAng, double dScaleX = 1.0, double dScaleY = 1.0)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                if (dScaleX == 1 && dScaleY == 1)
                {
                    ret = LMC1_SETROTATEMOVEPARAM(dMoveX, dMoveY, dCenX, dCenY, dRotateAng);
                }
                else
                {
                    ret = LMC1_SETROTATEMOVEPARAM2(dMoveX, dMoveY, dCenX, dCenY, dRotateAng, dScaleX, dScaleY);
                }
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        #endregion

        #region 标刻

        /// <summary>
        /// 标刻所有内容   飞动打标一定不能开启,否则会什么都不处理bFlyMark要保持false
        /// 加载模板后,,开关红光,然后调这个方法有效能打印.注意调这个方法后要手动调整设备的镜片高度进行焦点调整才行. 而调用MarkEnt 在现场测试没反应
        /// </summary>
        /// <param name="bFlyMark">true-飞动打标</param>
        /// <returns></returns>
        public static bool Mark(bool bFlyMark = false)
        {
            if (!mIsInitLaser)
            {
                MessageBox.Show(@"激光器没有初始化！");
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_MARK(bFlyMark);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 标刻指定对象
        /// </summary>
        /// <param name="strEntName"></param>
        /// <returns></returns>
        public static bool MarkEnt(string strEntName)
        {
            if (!mIsInitLaser)
            {
                MessageBox.Show(@"激光器没有初始化！");
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_MARKENTITY(strEntName);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;

            mLastError = ret;
    
                MessageBox.Show(mLastError.ToString());
           
            return false;
        }

        /// <summary>
        /// 停止标刻
        /// </summary>
        /// <returns></returns>
        public static bool StopMark()
        {
            if (!mIsInitLaser)
            {
                MessageBox.Show(@"激光器没有初始化");
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            var ret = LMC1_STOPMARK();

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 开始红光   调用后现场的设备镜片会出现闪动的红色矩形打印区域
        /// </summary>
        public static bool StartRed()
        {
            if (!mIsInitLaser)
            {
                MessageBox.Show(@"激光器没有初始化");
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            if (mThreadRedLight != null)
            {
                mThreadRedLight.Abort();
            }
            mEnableRed = true;
            mThreadRedLight = new Thread(RedLight);
            mThreadRedLight.Start();
            return true;
        }

        private static void RedLight()
        {
            while (mEnableRed)
            {
                lock (objectLock)
                {
                    var ret = LMC1_REDLIGHTMARK();
                    if (ret != LmcErrCode.LMC1_ERR_SUCCESS)
                    {
                        break;
                    }
                    Thread.Sleep(100);//这里会出现闪动
                }
            }
        }

        /// <summary>
        /// 停止红光 要停止红光后才能调用mark进行标刻
        /// </summary>
        public static void StopRed()
        {
            mEnableRed = false;
        }

        #endregion

        #region 文件

        /// <summary>
        /// 加载模板   bShowDialog设置为true时,会自动弹出一个文件模板选择界面,选择后直接加载覆盖第一个参数传的路径。看下面代码。
        /// </summary>
        /// <param name="strFile">模板路径</param>
        /// <param name="bShowDialog">是否显示加载界面</param>
        /// <returns></returns>
        public static bool LoadEzdFile(ref string strFile, bool bShowDialog = false)
        {
            if (!mIsInitLaser)
            {
                MessageBox.Show(@"激光器没有初始化");
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            if (bShowDialog)
            {
                var openFileDialog1 = new OpenFileDialog { Filter = @"模板文件(*.ezd)|*.ezd" };
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    strFile = openFileDialog1.FileName;
                }
                else
                {
                    return false;
                }
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_LOADEZDFILE(strFile);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
            {
                mMarkEzdFile = strFile;
                return true;
            }
            else
            {
                mLastError = ret;
                return false;
            }
        }

        /// <summary>
        /// 显示图片到Picture控件中
        /// </summary>
        /// <param name="pictureBox"></param>
        /// <param name="strEntName"></param>
        public static void ShowPreviewBmp(PictureBox pictureBox, string strEntName = null)
        {
            if (!mIsInitLaser)
            {
                MessageBox.Show(@"激光器没有初始化");
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return;
            }

            var width = pictureBox.Size.Width;
            var height = pictureBox.Size.Height;
            if (strEntName == null)
            {
                pictureBox.Invoke((EventHandler)(delegate
                {
                    lock (objectLock)
                    {
                        var ptr = LMC1_GETPREVBITMAP2(width, height);
                        pictureBox.Image = Image.FromHbitmap(ptr);
                    }
                }));
            }
            else
            {
                pictureBox.Invoke((EventHandler)(delegate
                {
                    lock (objectLock)
                    {
                        var ptr = LMC1_GETPREVBITMAPBYNAME2(strEntName, width, height);
                        pictureBox.Image = Image.FromHbitmap(ptr);
                    }
                }));
            }
        }

        /// <summary>
        /// 显示整个标刻区域图片到Picture控件中
        /// </summary>
        /// <param name="pictureBox"></param>
        public static void ShowPreviewBmp2(PictureBox pictureBox)
        {
            if (!mIsInitLaser)
            {
                MessageBox.Show(@"激光器没有初始化");
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return;
            }

            var minx = 0.0;
            var miny = 0.0;
            var maxx = 0.0;
            var maxy = 0.0;
            GetCoord(ref minx, ref miny, ref maxx, ref maxy);
            var width = (int)Math.Round((maxx - minx) / 100 * pictureBox.Size.Width, 3);
            var height = (int)Math.Round((maxy - miny) / 100 * pictureBox.Size.Height, 3);

            var coordX = (float)Math.Round((minx + 50) / 100 * pictureBox.Size.Width, 3);
            var coordY = (float)Math.Round((50 - maxy) / 100 * pictureBox.Size.Height, 3);
            Image previewImage;
            lock (objectLock)
            {
                var ptr = LMC1_GETPREVBITMAP2(width, height);
                previewImage = Image.FromHbitmap(ptr);
            }
            Image bitmap = new Bitmap(pictureBox.Size.Width, pictureBox.Size.Height);
            var g = Graphics.FromImage(bitmap);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.White);
            g.DrawImage(previewImage, new RectangleF(coordX, coordY, width, height), new RectangleF(0, 0, width, height), GraphicsUnit.Pixel);
            try
            {
                var ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                pictureBox.Invoke((EventHandler)delegate
                {
                    pictureBox.Image = Image.FromStream(ms);
                });
                ms.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                previewImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }


        }
        /// <summary>
        /// 保存模板
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool SaveEntLibToFile(string fileName = null)
        {
            if (!mIsInitLaser)
            {
                MessageBox.Show(@"激光器没有初始化");
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            if (fileName == null)
            {
                fileName = mMarkEzdFile;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_SAVEENTLIBTOFILE(fileName);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        #endregion

        #region 对象

        /// <summary>
        /// 获取整个模板的XY最大最小坐标
        /// </summary>
        /// <param name="dMinX"></param>
        /// <param name="dMinY"></param>
        /// <param name="dMaxX"></param>
        /// <param name="dMaxY"></param>
        /// <returns></returns>
        public static bool GetCoord(ref double dMinX, ref double dMinY, ref double dMaxX, ref double dMaxY)
        {
            var entCount = GetEntCount();
            for (var i = 0; i < entCount; i++)
            {
                var entName = string.Empty;
                if (!GetEntName(i, ref entName)) return false;
                var minx = 0.0;
                var maxx = 0.0;
                var miny = 0.0;
                var maxy = 0.0;
                var z = 0.0;
                if (!GetEntCoord(entName, ref minx, ref miny, ref maxx, ref maxy, ref z)) return false;
                if (i == 0)
                {
                    dMinX = minx;
                    dMaxX = maxx;
                    dMinY = miny;
                    dMaxY = maxy;
                }
                else
                {
                    if (minx < dMinX)
                        dMinX = minx;
                    if (miny < dMinY)
                        dMinY = miny;
                    if (maxx > dMaxX)
                        dMaxX = maxx;
                    if (maxy > dMaxY)
                        dMaxY = maxy;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取指定对象的XY最大最小坐标
        /// </summary>
        /// <param name="strEntName"></param>
        /// <param name="dMinX"></param>
        /// <param name="dMinY"></param>
        /// <param name="dMaxX"></param>
        /// <param name="dMaxY"></param>
        /// <param name="dZ"></param>
        /// <returns></returns>
        public static bool GetEntCoord(string strEntName, ref double dMinX, ref double dMinY, ref double dMaxX, ref double dMaxY, ref double dZ)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_GETENTSIZE(strEntName, ref dMinX, ref dMinY, ref dMaxX, ref dMaxY, ref dZ);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 根据对象名字获取中心坐标
        /// </summary>
        /// <param name="strEntName"></param>
        /// <param name="dCenx"></param>
        /// <param name="dCeny"></param>
        /// <returns></returns>
        public static bool GetEntCenter(string strEntName, ref double dCenx, ref double dCeny)
        {
            double minx = 0, miny = 0, maxx = 0, maxy = 0, z = 0;
            if (!GetEntCoord(strEntName, ref minx, ref miny, ref maxx, ref maxy, ref z))
            {
                return false;
            }
            dCenx = (maxx + minx) / 2;
            dCeny = (maxy + miny) / 2;
            return true;
        }

        /// <summary>
        /// 根据对象名字获取尺寸大小
        /// </summary>
        /// <param name="strEntName"></param>
        /// <param name="dWidth"></param>
        /// <param name="dHeight"></param>
        /// <returns></returns>
        public static bool GetEntSize(string strEntName, ref double dWidth, ref double dHeight)
        {
            double minx = 0, miny = 0, maxx = 0, maxy = 0, z = 0;
            if (!GetEntCoord(strEntName, ref minx, ref miny, ref maxx, ref maxy, ref z))
            {
                return false;
            }
            dWidth = maxx - minx;
            dHeight = maxy - miny;
            return true;
        }

        /// <summary>
        /// 将指定对象的中心坐标置零
        /// </summary>
        /// <param name="strEntName"></param>
        /// <returns></returns>
        public static bool SetEntCenterZero(string strEntName)
        {
            double minx = 0, miny = 0, maxx = 0, maxy = 0, z = 0;
            if (!GetEntCoord(strEntName, ref minx, ref miny, ref maxx, ref maxy, ref z))
            {
                return false;
            }
            var dx = (maxx + minx) / 2;
            var dy = (maxy + miny) / 2;

            return MoveEnt(strEntName, -dx, -dy);
        }

        /// <summary>
        /// 指定对象相对移动
        /// </summary>
        /// <param name="entName"></param>
        /// <param name="moveX"></param>
        /// <param name="moveY"></param>
        /// <returns></returns>
        public static bool MoveEnt(string entName, double moveX, double moveY)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_MOVEENT(entName, moveX, moveY);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 平移旋转所有对象
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static bool MoveAllEnt(double x, double y, double angle = 0)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            int entCount;
            lock (objectLock)
            {
                entCount = LMC1_GETENTITYCOUNT();
            }
            if (entCount < 1)
            {
                return false;
            }
            for (var i = 0; i < entCount; i++)
            {
                var strName = string.Empty;
                if (!GetEntName(i, ref strName))
                {
                    return false;
                }
                if (!SetEntName(i, "CurName"))
                {
                    return false;
                }
                if (angle != 0)
                {
                    double centerx = 0, centery = 0;
                    GetEntCenter("CurName", ref centerx, ref centery);
                    RotateEnt("CurName", centerx, centery, angle);
                }

                if (!MoveEnt("CurName", x, y))
                {
                    return false;
                }
                if (!SetEntName(i, strName))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 指定对象进行缩放
        /// </summary>
        /// <param name="strEntName"></param>
        /// <param name="dCenterX"></param>
        /// <param name="dCenterY"></param>
        /// <param name="dScaleX"></param>
        /// <param name="dScaleY"></param>
        /// <returns></returns>
        public static bool ScaleEnt(string strEntName, double dCenterX, double dCenterY, double dScaleX, double dScaleY)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_SCALEENT(strEntName, dCenterX, dCenterY, dScaleX, dScaleY);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 调整大小
        /// </summary>
        /// <param name="strEndName"></param>
        /// <param name="dToWidth"></param>
        /// <param name="dToHeight"></param>
        /// <returns></returns>
        public static bool ScaleEntSize(string strEndName, double dToWidth, double dToHeight)
        {
            double dMinx = 0, dMiny = 0, dMaxx = 0, dMaxy = 0, dz = 0;
            if (!GetEntCoord(strEndName, ref dMinx, ref dMiny, ref dMaxx, ref dMaxy, ref dz))
            {
                return false;
            }
            var dSizeX = dMaxx - dMinx;
            var dSizeY = dMaxy - dMiny;

            var dCenterX = (dMaxx + dMinx) / 2;
            var dCenterY = (dMaxy + dMiny) / 2;

            return ScaleEnt(strEndName, dCenterX, dCenterY, dToWidth / dSizeX, dToHeight / dSizeY);
        }

        /// <summary>
        /// 旋转指定对象
        /// </summary>
        /// <param name="strEntName"></param>
        /// <param name="dCenx"></param>
        /// <param name="dCeny"></param>
        /// <param name="dAngle"></param>
        /// <returns></returns>
        public static bool RotateEnt(string strEntName, double dCenx, double dCeny, double dAngle)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_ROTATEENT(strEntName, dCenx, dCeny, dAngle);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 获取对象个数
        /// </summary>
        /// <returns></returns>
        public static int GetEntCount()
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return 0;
            }
            int count;
            lock (objectLock)
            {
                count = LMC1_GETENTITYCOUNT();
            }
            return count;
        }

        /// <summary>
        /// 获取指定序号的对象名称
        /// </summary>
        /// <param name="nEntIndex"></param>
        /// <param name="strEntName"></param>
        /// <returns></returns>
        public static bool GetEntName(int nEntIndex, ref string strEntName)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            var chEnt = new char[256];
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_GETENTITYNAME(nEntIndex, chEnt);
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
            {
                strEntName = new string(chEnt);
                strEntName = strEntName.TrimEnd('\0');
                return true;
            }
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 设置指定序号的对象名称
        /// </summary>
        /// <param name="nEntIndex"></param>
        /// <param name="strEntName"></param>
        /// <returns></returns>
        public static bool SetEntName(int nEntIndex, string strEntName)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_SETENTITYNAME(nEntIndex, strEntName);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 自动设定未命名对象名称A1-An
        /// </summary>
        /// <returns></returns>
        public static bool AutoSetName()
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            var n = 1;
            var entCount = GetEntCount();
            for (var i = 0; i < entCount; i++)
            {
                var curName = string.Empty;
                if (!GetEntName(i, ref curName)) return false;
                if (curName == "")
                {
                    if (!SetEntName(i, "A" + n.ToString("D"))) return false;
                    n++;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取指定位图的参数
        /// </summary>
        /// <param name="EntName"></param>
        /// <param name="strImageFileName"></param>
        /// <param name="nBmpAttrib"></param>
        /// <param name="nScanAttrib"></param>
        /// <param name="dBrightness"></param>
        /// <param name="dContrast"></param>
        /// <param name="dPointTime"></param>
        /// <param name="nImportDpi"></param>
        /// <param name="bDisableMarkLowGrayPt"></param>
        /// <param name="nMinLowGrayPt"></param>
        /// <returns></returns>
        public static bool GetBitMapEntParam(
            string EntName,
            StringBuilder strImageFileName,
            ref int nBmpAttrib,
            ref int nScanAttrib,
            ref double dBrightness,
            ref double dContrast,
            ref double dPointTime,
            ref int nImportDpi,
            ref int bDisableMarkLowGrayPt,
            ref int nMinLowGrayPt)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_GETBITMAPENTPARAM(EntName, strImageFileName, ref nBmpAttrib, ref nScanAttrib, ref dBrightness, ref dContrast, ref dPointTime, ref nImportDpi);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 设置指定位图的参数
        /// </summary>
        /// <param name="EntName"></param>
        /// <param name="strImageFileName"></param>
        /// <param name="nBmpAttrib"></param>
        /// <param name="nScanAttrib"></param>
        /// <param name="dBrightness"></param>
        /// <param name="dContrast"></param>
        /// <param name="dPointTime"></param>
        /// <param name="nImportDpi"></param>
        /// <returns></returns>
        public static bool SetBitMapEntParam2(
            string EntName,
            string strImageFileName,
            int nBmpAttrib,
            int nScanAttrib,
            double dBrightness,
            double dContrast,
            double dPointTime,
            int nImportDpi)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_SETBITMAPENTPARAM2(EntName, strImageFileName, nBmpAttrib, nScanAttrib, dBrightness, dContrast, dPointTime, nImportDpi);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        #endregion

        #region 端口

        /// <summary>
        /// 读取输入端口信号
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        public static bool ReadPort(int nPort)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            var nState = 0;
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_READPORT(ref nState);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return ((nState >> nPort) & 0x01) > 0;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 读取输入端口信号
        /// </summary>
        /// <param name="nStatus"></param>
        /// <returns></returns>
        public static bool ReadPort(ref int[] nStatus)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            for (var i = 0; i < nStatus.Length; i++)
            {
                nStatus[i] = 0;
            }

            var nSts = 0;
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_READPORT(ref nSts);
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
            {
                for (var i = 0; i < nStatus.Length; i++)
                {
                    nStatus[i] = ((nSts >> i) & 0x01);
                }
                return true;
            }
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 读取输出端口信号
        /// </summary>
        /// <param name="nStatus"></param>
        /// <returns></returns>
        public static bool GetOutPort(ref int[] nStatus)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            for (var i = 0; i < nStatus.Length; i++)
            {
                nStatus[i] = 0;
            }

            var nSts = 0;
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_GETOUTPORT(ref nSts);
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
            {
                for (var i = 0; i < nStatus.Length; i++)
                {
                    nStatus[i] = (nSts >> i) & 0x01;
                }
                return true;
            }
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 写入输出端口信号
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="bState"></param>
        /// <returns></returns>
        public static bool WritePort(int nPort, bool bState)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            if (nPort < 0 || nPort > 15)
            {
                return false;
            }
            var nState = 0;
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_GETOUTPORT(ref nState);
            }
            if (ret != LmcErrCode.LMC1_ERR_SUCCESS)
            {
                mLastError = ret;
                return false;
            }

            int debuff;
            if (bState)
            {
                debuff = 0x0001 << nPort;
                nState |= debuff;
            }
            else
            {
                debuff = ~(0x0001 << nPort);
                nState &= debuff;
            }

            lock (objectLock)
            {
                ret = LMC1_WRITEPORT(nState);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        #endregion

        #region 文本

        /// <summary>
        /// 更改指定名称的文本对象的文本内容
        /// </summary>
        /// <param name="strEntName"></param>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static bool ChangeTextByName(string strEntName, string strText)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_CHANGETEXTBYNAME(strEntName, strText);
            }
            if (ret != LmcErrCode.LMC1_ERR_SUCCESS)
            {
                mLastError = ret;
                return false;
            }

            var text = string.Empty;
            var res = GetTextByName(strEntName, ref text);
            if (!res || text != strText)
                return false;
            return true;
        }

        /// <summary>
        /// 获取指定名称的文本对象的文本内容
        /// </summary>
        /// <param name="strEntName"></param>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static bool GetTextByName(string strEntName, ref string strText)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            var sz = new char[256];
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_GETTEXTBYNAME(strEntName, sz);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
            {
                strText = new string(sz);
                strText = strText.TrimEnd('\0');
                return true;
            }

            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 显示所有对象名字到ListView中
        /// </summary>
        /// <param name="listView"></param>
        public static void ShowAllEntToList(ListView listView)
        {
            var nCount = GetEntCount();
            if (nCount <= 0)
            {
                return;
            }

            listView.Invoke((EventHandler)(delegate
            {
                listView.BeginUpdate();
                listView.Items.Clear();
                for (var i = 0; i < nCount; i++)
                {
                    var strEntName = string.Empty;
                    GetEntName(i, ref strEntName);

                    var lvItem = new ListViewItem { Text = (i + 1).ToString() };
                    listView.Items.Add(lvItem);
                    lvItem.SubItems.Add(strEntName);
                }
                listView.EndUpdate();
            }));
        }

        #endregion

        #region 笔号

        /// <summary>
        /// 获取对应笔号的参数
        /// </summary>
        /// <param name="nPenNo"></param>
        /// <param name="nMarkLoop"></param>
        /// <param name="dMarkSpeed"></param>
        /// <param name="dPowerRatio"></param>
        /// <param name="dCurrent"></param>
        /// <param name="nFreq"></param>
        /// <param name="dQPulseWidth"></param>
        /// <param name="nStartTC"></param>
        /// <param name="nLaserOffTC"></param>
        /// <param name="nEndTC"></param>
        /// <param name="nPolyTC"></param>
        /// <param name="dJumpSpeed"></param>
        /// <param name="nJumpPosTC"></param>
        /// <param name="nJumpDistTC"></param>
        /// <param name="dEndComp"></param>
        /// <param name="dAccDist"></param>
        /// <param name="dPointTime"></param>
        /// <param name="bPulsePointMode"></param>
        /// <param name="nPulseNum"></param>
        /// <param name="dFlySpeed"></param>
        /// <returns></returns>
        public static bool GetPenParam(
            int nPenNo,                 //要设置的笔号(0-255)					 
            ref int nMarkLoop,          //加工次数
            ref double dMarkSpeed,      //标刻次数mm/s
            ref double dPowerRatio,     //功率百分比(0-100%)	
            ref double dCurrent,        //电流A
            ref int nFreq,              //频率HZ
            ref double dQPulseWidth,    //Q脉冲宽度us	
            ref int nStartTC,           //开始延时us
            ref int nLaserOffTC,        //激光关闭延时us 
            ref int nEndTC,             //结束延时us
            ref int nPolyTC,            //拐角延时us   	
            ref double dJumpSpeed,      //跳转速度mm/s
            ref int nJumpPosTC,         //跳转位置延时us
            ref int nJumpDistTC,        //跳转距离延时us	
            ref double dEndComp,        //末点补偿mm
            ref double dAccDist,        //加速距离mm	
            ref double dPointTime,      //打点延时 ms						 
            ref bool bPulsePointMode,   //脉冲点模式    
            ref int nPulseNum,          //脉冲点数目
            ref double dFlySpeed)       //流水线速度
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_GETPENPARAM(nPenNo, ref nMarkLoop,
                    ref dMarkSpeed,
                    ref dPowerRatio,
                    ref dCurrent,
                    ref nFreq,
                    ref dQPulseWidth,
                    ref nStartTC,
                    ref nLaserOffTC,
                    ref nEndTC,
                    ref nPolyTC,
                    ref dJumpSpeed,
                    ref nJumpPosTC,
                    ref nJumpDistTC,
                    ref dEndComp,
                    ref dAccDist,
                    ref dPointTime,
                    ref bPulsePointMode,
                    ref nPulseNum,
                    ref dFlySpeed);
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 获取对应索引的当前数据信息
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="dPowerRatio"></param>
        /// <param name="nFreq"></param>
        /// <param name="dCurrent"></param>
        /// <returns></returns>
        public static bool GetParamByName(int nIndex, ref double dPowerRatio, ref int nFreq, ref double dCurrent)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            var strEntName = string.Empty;
            if (!GetEntName(nIndex, ref strEntName))
            {
                return false;
            }
            if (!SetEntName(nIndex, "CurName"))
            {
                return false;
            }
            int nPen;
            lock (objectLock)
            {
                nPen = LMC1_GETPENNUMBERFROMENT("CurName");
            }
            if (!SetEntName(nIndex, strEntName))
            {
                return false;
            }
            if (nPen < 0)
            {
                return false;
            }
            int nMarkLoop = 0;
            double dMarkSpeed = 0;
            double dQPulseWidth = 0;
            int nStartTC = 0;
            int nLaserOffTC = 0;
            int nEndTC = 0;
            int nPolyTC = 0;
            double dJumpSpeed = 0;
            int nJumpPosTC = 0;
            int nJumpDistTC = 0;
            double dEndComp = 0;
            double dAccDist = 0;
            double dPointTime = 0;
            bool bPulsePointMode = false;
            int nPulseNum = 0;
            double dFlySpeed = 0;

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_GETPENPARAM(nPen, ref nMarkLoop, ref dMarkSpeed, ref dPowerRatio, ref dCurrent, ref nFreq, ref dQPulseWidth,
                    ref nStartTC, ref nLaserOffTC, ref nEndTC, ref nPolyTC, ref dJumpSpeed, ref nJumpPosTC, ref nJumpDistTC, ref dEndComp
                    , ref dAccDist, ref dPointTime, ref bPulsePointMode, ref nPulseNum, ref dFlySpeed);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 设置对应笔号的参数
        /// </summary>
        /// <param name="nPenNo"></param>
        /// <param name="nMarkLoop"></param>
        /// <param name="dMarkSpeed"></param>
        /// <param name="dPowerRatio"></param>
        /// <param name="dCurrent"></param>
        /// <param name="nFreq"></param>
        /// <param name="dQPulseWidth"></param>
        /// <param name="nStartTC"></param>
        /// <param name="nLaserOffTC"></param>
        /// <param name="nEndTC"></param>
        /// <param name="nPolyTC"></param>
        /// <param name="dJumpSpeed"></param>
        /// <param name="nJumpPosTC"></param>
        /// <param name="nJumpDistTC"></param>
        /// <param name="dEndComp"></param>
        /// <param name="dAccDist"></param>
        /// <param name="dPointTime"></param>
        /// <param name="bPulsePointMode"></param>
        /// <param name="nPulseNum"></param>
        /// <param name="dFlySpeed"></param>
        /// <returns></returns>
        public static bool SetPenParam(
            int nPenNo,                 //要设置的笔号(0-255)
            int nMarkLoop,              //加工次数
            double dMarkSpeed,          //标刻次数mm/s
            double dPowerRatio,         //功率百分比(0-100%)
            double dCurrent,            //电流A
            int nFreq,                  //频率HZ
            double dQPulseWidth,        //Q脉冲宽度us	
            int nStartTC,               //开始延时us
            int nLaserOffTC,            //激光关闭延时us 
            int nEndTC,                 //结束延时us
            int nPolyTC,                //拐角延时us
            double dJumpSpeed,          //跳转速度mm/s
            int nJumpPosTC,             //跳转位置延时us
            int nJumpDistTC,            //跳转距离延时us	
            double dEndComp,            //末点补偿mm
            double dAccDist,            //加速距离mm	    
            double dPointTime,          //打点延时 ms		
            bool bPulsePointMode,       //脉冲点模式 
            int nPulseNum,              //脉冲点数目
            double dFlySpeed)           //流水线速度
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_SETPENPARAM(nPenNo, nMarkLoop, dMarkSpeed, dPowerRatio, dCurrent, nFreq,
                     dQPulseWidth, nStartTC, nLaserOffTC, nEndTC, nPolyTC, dJumpSpeed, nJumpPosTC,
                     nJumpDistTC, dEndComp, dAccDist, dPointTime, bPulsePointMode, nPulseNum, dFlySpeed);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 根据对象索引设置功率，频率，电流
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="dSetPower"></param>
        /// <param name="nSetRate"></param>
        /// <param name="dSetCurrent"></param>
        /// <returns></returns>
        public static bool SetParamByName(int nIndex, double dSetPower, int nSetRate, double dSetCurrent)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            if (dSetPower < 0 || dSetPower > 100)
            {
                MessageBox.Show(@"功率超过实际范围");
                return false;
            }
            var strEntName = string.Empty;
            if (!GetEntName(nIndex, ref strEntName))
            {
                return false;
            }
            if (!SetEntName(nIndex, "CurName"))
            {
                return false;
            }
            int nPen;
            lock (objectLock)
            {
                nPen = LMC1_GETPENNUMBERFROMENT("CurName");
            }
            if (!SetEntName(nIndex, strEntName))
            {
                return false;
            }
            if (nPen < 0)
            {
                return false;
            }
            int nMarkLoop = 0;
            double dMarkSpeed = 0;
            double dPowerRatio = 0;
            double dCurrent = 0;
            int nFreq = 0;
            double dQPulseWidth = 0;
            int nStartTC = 0;
            int nLaserOffTC = 0;
            int nEndTC = 0;
            int nPolyTC = 0;
            double dJumpSpeed = 0;
            int nJumpPosTC = 0;
            int nJumpDistTC = 0;
            double dEndComp = 0;
            double dAccDist = 0;
            double dPointTime = 0;
            bool bPulsePointMode = false;
            int nPulseNum = 0;
            double dFlySpeed = 0;
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_GETPENPARAM(nPen, ref nMarkLoop, ref dMarkSpeed, ref dPowerRatio, ref dCurrent, ref nFreq, ref dQPulseWidth,
                    ref nStartTC, ref nLaserOffTC, ref nEndTC, ref nPolyTC, ref dJumpSpeed, ref nJumpPosTC, ref nJumpDistTC, ref dEndComp
                    , ref dAccDist, ref dPointTime, ref bPulsePointMode, ref nPulseNum, ref dFlySpeed);
            }
            if (ret != LmcErrCode.LMC1_ERR_SUCCESS)
            {
                return false;
            }

            lock (objectLock)
            {
                ret = LMC1_SETPENPARAM(nPen, nMarkLoop, dMarkSpeed, dSetPower, dSetCurrent, nSetRate, dQPulseWidth,
                    nStartTC, nLaserOffTC, nEndTC, nPolyTC, dJumpSpeed, nJumpPosTC, nJumpDistTC, dEndComp
                    , dAccDist, dPointTime, bPulsePointMode, nPulseNum, dFlySpeed);
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        #endregion

        #region 添加删除对象

        /// <summary>
        /// 清除对象库里所有对象
        /// </summary>
        /// <returns></returns>
        public static bool ClearEntLib()
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_CLEARENTLIB();
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 删除指定对象名字
        /// </summary>
        /// <param name="strEntName"></param>
        /// <returns></returns>
        public static bool DeleteEnt(string strEntName)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_DELETEENTLIB(strEntName);
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 添加新文本到模板中
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strEntName"></param>
        /// <param name="dLeftDownXPos"></param>
        /// <param name="dLeftDownYPos"></param>
        /// <param name="dZpos"></param>
        /// <param name="nAlign"></param>
        /// <returns></returns>
        public static bool AddTextToLib(string strName, string strEntName, double dLeftDownXPos, double dLeftDownYPos, double dZpos, int nAlign)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_ADDTEXTTOLIB(strName, strEntName, dLeftDownXPos, dLeftDownYPos, dZpos, nAlign, 0, 0, false);
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 添加文件到模板中
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="strEntName"></param>
        /// <param name="dLeftDownXPos"></param>
        /// <param name="dLeftDowmYPos"></param>
        /// <param name="dZpos"></param>
        /// <param name="nAlign"></param>
        /// <returns></returns>
        public static bool AddFileToLib(string strFilePath, string strEntName, double dLeftDownXPos, double dLeftDowmYPos, double dZpos, int nAlign)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_ADDFILETOLIB(strFilePath, strEntName, dLeftDownXPos, dLeftDowmYPos, dZpos, nAlign, 1, 0, false);
            }

            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
                return true;
            mLastError = ret;
            return false;
        }

        #endregion

        #region 扩展轴

        /// <summary>
        /// 使能复位
        /// </summary>
        /// <param name="bEnAxis0"></param>
        /// <param name="bEnAxis1"></param>
        /// <returns></returns>
        public static bool AxisEnable(bool bEnAxis0, bool bEnAxis1)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_RESET(bEnAxis0, bEnAxis1);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
            {
                if (bEnAxis0)
                {
                    mEnableAxis0 = true;
                }
                if (bEnAxis1)
                {
                    mEnableAxis1 = true;
                }
                return true;
            }

            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool AxisGoHome(int axis)
        {
            if (axis < 0 || axis > 1)
            {
                return false;
            }
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_AXISCORRECTORIGION(axis);
            }
            if (ret == LmcErrCode.LMC1_ERR_SUCCESS)
            {
                if (axis == 0)
                {
                    mHomeFlagAxis0 = true;
                }
                if (axis == 1)
                {
                    mHomeFlagAxis1 = true;
                }
                return true;
            }

            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 绝对移动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="goalPos"></param>
        /// <returns></returns>
        public static bool AxisAbsMove(int axis, double goalPos)
        {
            if (axis < 0 || axis > 1)
            {
                return false;
            }
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            LmcErrCode ret;
            lock (objectLock)
            {
                ret = LMC1_AXISMOVETO(axis, goalPos);
            }
            if (LmcErrCode.LMC1_ERR_SUCCESS == ret)
            {
                return true;
            }

            mLastError = ret;
            return false;
        }

        /// <summary>
        /// 相对移动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static bool AxisRelMove(int axis, double distance)
        {
            if (axis < 0 || axis > 1)
            {
                return false;
            }
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }

            var curPos = GetAxisCoord(axis);
            var goalPos = curPos + distance;
            return AxisAbsMove(axis, goalPos);
        }

        /// <summary>
        /// 连续移动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="isPlusDir"></param>
        public static void AxisJogMove(int axis, bool isPlusDir)
        {
            if (axis < 0 || axis > 1)
            {
                return;
            }
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return;
            }

            mIsJogMoving = true;
            while (mIsJogMoving)
            {
                int dir;
                if (isPlusDir)
                {
                    dir = 1;
                }
                else
                {
                    dir = -1;
                }
                AxisRelMove(axis, dir * 0.1);
            }
        }

        /// <summary>
        /// 停止连续运动
        /// </summary>
        public static void AxisJogStop()
        {
            mIsJogMoving = false;
        }

        /// <summary>
        /// 获取当前坐标
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static double GetAxisCoord(int axis)
        {
            if (axis < 0 || axis > 1)
            {
                return 0;
            }
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return 0;
            }

            double ret;
            lock (objectLock)
            {
                ret = LMC1_GETAXISCOOR(axis);
            }
            return ret;
        }

        #endregion

        #region 其他

        /// <summary>
        /// 获取当前错误
        /// </summary>
        /// <returns></returns>
        public static string GetLastError()
        {
            string lastError;
            switch (mLastError)
            {
                case LmcErrCode.LMC1_ERR_SUCCESS:
                    lastError = "成功";
                    break;
                case LmcErrCode.LMC1_ERR_EZCADRUN:
                    lastError = "发现EZCAD在运行";
                    break;
                case LmcErrCode.LMC1_ERR_NOFINDCFGFILE:
                    lastError = "找不到EZCAD.CFG";
                    break;
                case LmcErrCode.LMC1_ERR_FAILEDOPEN:
                    lastError = "打开LMC1失败";
                    break;
                case LmcErrCode.LMC1_ERR_NODEVICE:
                    lastError = "没有有效的lmc1设备";
                    break;
                case LmcErrCode.LMC1_ERR_HARDVER:
                    lastError = "lmc1版本错误";
                    break;
                case LmcErrCode.LMC1_ERR_DEVCFG:
                    lastError = "找不到设备配置文件";
                    break;
                case LmcErrCode.LMC1_ERR_STOPSIGNAL:
                    lastError = "报警信号";
                    break;
                case LmcErrCode.LMC1_ERR_USERSTOP:
                    lastError = "用户停止";
                    break;
                case LmcErrCode.LMC1_ERR_UNKNOW:
                    lastError = "不明错误";
                    break;
                case LmcErrCode.LMC1_ERR_OUTTIME:
                    lastError = "超时";
                    break;
                case LmcErrCode.LMC1_ERR_NOINITIAL:
                    lastError = "未初始化";
                    break;
                case LmcErrCode.LMC1_ERR_READFILE:
                    lastError = "读文件错误";
                    break;
                case LmcErrCode.LMC1_ERR_OWENWNDNULL:
                    lastError = "窗口为空";
                    break;
                case LmcErrCode.LMC1_ERR_NOFINDFONT:
                    lastError = "找不到指定名称的字体";
                    break;
                case LmcErrCode.LMC1_ERR_PENNO:
                    lastError = "错误的笔号";
                    break;
                case LmcErrCode.LMC1_ERR_NOTTEXT:
                    lastError = "指定名称的对象不是文本对象";
                    break;
                case LmcErrCode.LMC1_ERR_SAVEFILE:
                    lastError = "保存文件失败";
                    break;
                case LmcErrCode.LMC1_ERR_NOFINDENT:
                    lastError = "找不到指定对象";
                    break;
                case LmcErrCode.LMC1_ERR_STATUE:
                    lastError = "当前状态下不能执行此操作";
                    break;
                case LmcErrCode.LMC1_ERR_PARAM1:
                    lastError = "错误的执行参数";
                    break;
                case LmcErrCode.LMC1_ERR_PARAM2:
                    lastError = "错误的硬件参数";
                    break;
                case LmcErrCode.LMC1_ERR_NOFINDEZD:
                    lastError = "未找到MarkEzd.dll文件";
                    break;
                default:
                    lastError = "无法识别的错误" + mLastError.ToString();
                    break;
            }
            return lastError;
        }

        /// <summary>
        /// 打开标准软件EzCad2.exe
        /// </summary>
        /// <returns></returns>
        public static bool OpenEzCad2()
        {
            if (mThreadInitLaser != null)
            {
                if (mThreadInitLaser.IsAlive)
                {
                    return false;
                }
                mThreadInitLaser.Abort();
            }
            if (!File.Exists(Application.StartupPath + "\\" + "EzCad2.exe"))
            {
                MessageBox.Show(@"EzCad2.exe不存在！");
                return false;
            }

            mThreadInitLaser = new Thread(threadInitHglaser);
            mThreadInitLaser.Start();
            return true;
        }

        private static void threadInitHglaser()
        {
            Close();
            mIsInitLaser = false;
            var process = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = Application.StartupPath + @"\EzCad2.exe",
                    Arguments = mMarkEzdFile,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            try
            {
                process.Start();
            }
            catch (System.ComponentModel.Win32Exception we)
            {
                process.Dispose();
                MessageBox.Show(we.Message);
                return;
            }
            process.WaitForExit();
            process.Dispose();
            if (!InitLaser())
            {
                MessageBox.Show(@"激光器初始化失败！！");
            }
            else
            {
                MessageBox.Show(@"激光器初始化成功，请重新加载模板！");
            }
        }

        #endregion

    }
}
