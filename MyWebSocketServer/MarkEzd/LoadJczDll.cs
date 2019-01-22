using System;
using System.Runtime.InteropServices;
using System.Text;

namespace JczMark
{
    public class LoadJczDll
    {

        #region 通用错误码

        public enum LmcErrCode
        {
            LMC1_ERR_SUCCESS = 0,           //成功
            LMC1_ERR_EZCADRUN = 1,          //发现EZCAD在运行
            LMC1_ERR_NOFINDCFGFILE = 2,     //找不到EZCAD.CFG
            LMC1_ERR_FAILEDOPEN = 3,        //打开LMC1失败
            LMC1_ERR_NODEVICE = 4,          //没有有效的lmc1设备
            LMC1_ERR_HARDVER = 5,           //lmc1版本错误
            LMC1_ERR_DEVCFG = 6,            //找不到设备配置文件
            LMC1_ERR_STOPSIGNAL = 7,        //报警信号
            LMC1_ERR_USERSTOP = 8,          //用户停止
            LMC1_ERR_UNKNOW = 9,            //不明错误
            LMC1_ERR_OUTTIME = 10,          //超时
            LMC1_ERR_NOINITIAL = 11,        //未初始化
            LMC1_ERR_READFILE = 12,         //读文件错误
            LMC1_ERR_OWENWNDNULL = 13,      //窗口为空
            LMC1_ERR_NOFINDFONT = 14,       //找不到指定名称的字体
            LMC1_ERR_PENNO = 15,            //错误的笔号
            LMC1_ERR_NOTTEXT = 16,          //指定名称的对象不是文本对象
            LMC1_ERR_SAVEFILE = 17,         //保存文件失败
            LMC1_ERR_NOFINDENT = 18,        //找不到指定对象
            LMC1_ERR_STATUE = 19,           //当前状态下不能执行此操作
            LMC1_ERR_PARAM1 = 20,           //错误的执行参数
            LMC1_ERR_PARAM2 = 21,           //错误的硬件参数
            LMC1_ERR_NOFINDEZD = 22,
        }

        #endregion

        #region 设备

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Initial")]
        protected static extern LmcErrCode LMC1_INITIAL(string strEzCadPath, bool bTestMode, IntPtr hOwenWnd);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Initial2")]
        protected static extern LmcErrCode LMC1_INITIAL2(string strEzCadPath, bool bTestMode);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Close")]
        protected static extern LmcErrCode LMC1_CLOSE();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetDevCfg")]
        protected static extern LmcErrCode LMC1_SETDEVCFG();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetDevCfg2")]
        protected static extern LmcErrCode LMC1_SETDEVCFG2(bool bAxisShow0, bool bAxisShow1);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetRotateMoveParam")]
        protected static extern LmcErrCode LMC1_SETROTATEMOVEPARAM(double dMoveX, double dMoveY, double dCenterX, double dCenterY, double dRotateAng);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetRotateMoveParam2")]
        protected static extern LmcErrCode LMC1_SETROTATEMOVEPARAM2(double dMoveX, double dMoveY, double dCenterX, double dCenterY, double dRotateAng, double dScaleX, double dScaleY);

        #endregion

        #region 标刻

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Mark")]
        protected static extern LmcErrCode LMC1_MARK(bool bFlyMark);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_MarkEntity")]
        protected static extern LmcErrCode LMC1_MARKENTITY(string strEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_MarkFlyByStartSignal")]
        protected static extern LmcErrCode LMC1_MARKFLYBYSTARTSIGNAL();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_MarkEntityFly")]
        protected static extern LmcErrCode LMC1_MARKENTITYFLY(string strEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_MarkLine")]
        protected static extern LmcErrCode LMC1_MARKLINE(double x1, double y1, double x2, double y2, int pen);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_MarkPoint")]
        protected static extern LmcErrCode LMC1_MARKPOINT(double x, double y, double delay, int pen);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ MarkPointBuf2")]
        protected static extern LmcErrCode LMC1_MARKPOINTBUF2(double[][] ptBuf, double dJumpSpeed, double dLaserOnTimeMs);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_IsMarking")]
        protected static extern LmcErrCode LMC1_ISMARKING();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_StopMark")]
        protected static extern LmcErrCode LMC1_STOPMARK();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_RedLightMark")]
        protected static extern LmcErrCode LMC1_REDLIGHTMARK();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_RedLightMark2")]
        protected static extern LmcErrCode LMC1_REDLIGHTMARK2();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ RedLightMarkContour")]
        protected static extern LmcErrCode LMC1_REDLIGHTMARKCONTOUR();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ RedLightMarkContour2")]
        protected static extern LmcErrCode LMC1_REDLIGHTMARKCONTOUR2();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ RedLightMarkByEnt")]
        protected static extern LmcErrCode LMC1_REDLIGHTMARKBYENT(string strEntName, bool bContour);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ GetFlySpeed")]
        protected static extern LmcErrCode LMC1_GETFLYSPEED(ref double flySpeed);

        #endregion

        #region 文件

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_LoadEzdFile")]
        protected static extern LmcErrCode LMC1_LOADEZDFILE(string strFileName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPrevBitmap")]
        protected static extern IntPtr LMC1_GETPREVBITMAP(IntPtr hwnd, int nBMPWIDTH, int nBMPHEIGHT);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPrevBitmap2")]
        protected static extern IntPtr LMC1_GETPREVBITMAP2(int nBMPWIDTH, int nBMPHEIGHT);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPrevBitmapByName2")]
        protected static extern IntPtr LMC1_GETPREVBITMAPBYNAME2(string strEntName, int nBMPWIDTH, int nBMPHEIGHT);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPrevBitmap3")]
        protected static extern IntPtr LMC1_GETPREVBITMAP3(int nBMPWIDTH, int nBMPHEIGHT, double dLogOriginX, double dLogOriginY, double dLogScale);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SaveEntLibToFile")]
        protected static extern LmcErrCode LMC1_SAVEENTLIBTOFILE(string strFileName);

        #endregion

        #region 对象

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntSize")]
        protected static extern LmcErrCode LMC1_GETENTSIZE(string strEntName, ref double dMinx, ref double dMiny, ref double dMaxx, ref double dMaxy, ref double dZ);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_MoveEnt")]
        protected static extern LmcErrCode LMC1_MOVEENT(string pEntName, double dMovex, double dMovey);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ScaleEnt")]
        protected static extern LmcErrCode LMC1_SCALEENT(string pEntName, double dCenX, double dCeny, double dScalex, double dScaley);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_MirrorEnt")]
        protected static extern LmcErrCode LMC1_MIRRORENT(string pEntName, double dCenX, double dCeny, bool bMirrorX, bool bMirrorY);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_RotateEnt")]
        protected static extern LmcErrCode LMC1_ROTATEENT(string strEntName, double dCenx, double dCeny, double dAngle);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_CopyEnt")]
        protected static extern LmcErrCode LMC1_COPYENT(string pEntName, string pNewEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntityCount")]
        protected static extern int LMC1_GETENTITYCOUNT();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntityName")]
        protected static extern LmcErrCode LMC1_GETENTITYNAME(int nEntityIndex, char[] strEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetEntityName")]
        protected static extern LmcErrCode LMC1_SETENTITYNAME(int nEntityIndex, string pEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ChangeEntName")]
        protected static extern LmcErrCode LMC1_CHANGEENTNAME(string pEntName, string pNewEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GroupEnt")]
        protected static extern LmcErrCode LMC1_GROUPENT(string pEntName1, string pEntName2, string pEntNameNew, int pen);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_UnGroupEnt")]
        protected static extern LmcErrCode LMC1_UNGROUPENT(string pGroupEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ GetBitmapEntParam")]
        protected static extern LmcErrCode LMC1_GETBITMAPENTPARAM(string strEntName, StringBuilder BmpPath, ref int nBmpAttrib, ref int nScanAttrib, ref double dBrightness, ref double dContrast, ref double dPointTime, ref int nImportDpi);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ SetBitmapEntParam2")]
        protected static extern LmcErrCode LMC1_SETBITMAPENTPARAM2(string strEntName, string strBmpPath, int nBmpAttrib, int nScanAttrib, double dBrightness, double dContrast, double dPointTime, int nImportDpi);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ MoveEntityBefore")]
        protected static extern LmcErrCode LMC1_MOVEENTITYBEFORE(int nMoveEnt, int nGoalEnt);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ MoveEntityAfter")]
        protected static extern LmcErrCode LMC1_MOVEENTITYAFTER(int nMoveEnt, int nGoalEnt);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ ReverseAllEntOrder")]
        protected static extern LmcErrCode LMC1_REVERSEALLENTORDER();

        #endregion

        #region 端口

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ReadPort")]
        protected static extern LmcErrCode LMC1_READPORT(ref int nData);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_WritePort")]
        protected static extern LmcErrCode LMC1_WRITEPORT(int nData);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetOutPort")]
        protected static extern LmcErrCode LMC1_GETOUTPORT(ref int nData);

        #endregion

        #region 文本

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ChangeTextByName")]
        protected static extern LmcErrCode LMC1_CHANGETEXTBYNAME(string strEntName, string strText);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetTextByName")]
        protected static extern LmcErrCode LMC1_GETTEXTBYNAME(string strName, char[] strText);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_TextResetSn")]
        protected static extern LmcErrCode LMC1_TEXTRESETSN(string pTextName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ EntityIsBarcode")]
        protected static extern LmcErrCode LMC1_ENTITYISBARCODE(string pTextName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ IsVarText")]
        protected static extern LmcErrCode LMC1_ISVARTEXT(string pTextName, ref bool bVar);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetAllFontRecord")]
        protected static extern LmcErrCode LMC1_GETALLFONTRECORD(ref int nFontNum);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetFontParam")]
        protected static extern LmcErrCode LMC1_SETFONTPARAM(string strFontName, double dCharHeight, double dCharWidth, double dCharAngle, double dCharSpace, double dLineSpace, bool bEqualCharWidth);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetTextEntParam")]
        protected static extern LmcErrCode LMC1_SETTEXTENTPARAM(string strTextName, double dCharHeight, double dCharWidth, double dCharAngle, double dCharSpace, double dLineSpace, bool bEqualCharWidth);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetTextEntParam2")]
        protected static extern LmcErrCode LMC1_SETTEXTENTPARAM2(string strTextName, string strFontName, double dCharHeight, double dCharWidth, double dCharAngle, double dCharSpace, double dLineSpace, double dSpaceWidth, bool bEqualCharWidth);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetTextEntParam")]
        protected static extern LmcErrCode LMC1_GETTEXTENTPARAM(string strTextName, char[] sFontName, ref double dCharHeight, ref double dCharWidth, ref double dCharAngle, ref double dCharSpace, ref double dLineSpace, ref bool bEqualCharWidth);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetTextEntParam2")]
        protected static extern LmcErrCode LMC1_GETTEXTENTPARAM2(string strTextName, char[] sFontName, ref double dCharHeight, ref double dCharWidth, ref double dCharAngle, ref double dCharSpace, ref double dLineSpace, ref double dSpaceWidth, ref bool bEqualCharWidth);

        #endregion

        #region 笔号

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPenParam")]
        protected static extern LmcErrCode LMC1_GETPENPARAM(
            int nPenNo,                 //要设置的笔号(0-255)					 
            ref int nMarkLoop,          //加工次数
            ref double dMarkSpeed,      //标刻次数mm/s
            ref double dPowerRatio,     //功率百分比(0-100%)	
            ref double dCurrent,        //电流A
            ref int nFreq,              //频率HZ
            ref double nQPulseWidth,    //Q脉冲宽度us	
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
            ref double dFlySpeed);      //流水线速度s

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPenParam2")]
        protected static extern LmcErrCode LMC1_GETPENPARAM2(
            int nPenNo,                 //要设置的笔号(0-255)					 
            ref int nMarkLoop,          //加工次数
            ref double dMarkSpeed,      //标刻次数mm/s
            ref double dPowerRatio,     //功率百分比(0-100%)	
            ref double dCurrent,        //电流A
            ref int nFreq,              //频率HZ
            ref double nQPulseWidth,    //Q脉冲宽度us	
            ref int nStartTC,           //开始延时us
            ref int nLaserOffTC,        //激光关闭延时us 
            ref int nEndTC,             //结束延时us
            ref int nPolyTC,            //拐角延时us   	
            ref double dJumpSpeed,      //跳转速度mm/s
            ref int nJumpPosTC,         //跳转位置延时us
            ref int nJumpDistTC,        //跳转距离延时us	
            ref double dPointTime,      //打点时间
            ref int nSpiWave,           //SPI波形选择	
            ref bool bWobbleMode,       //抖动模式						 
            ref double bWobbleDiameter, //抖动直径 
            ref double bWobbleDist);    //抖动间距


        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPenParam4")]
        protected static extern LmcErrCode LMC1_GETPENPARAM4(
            int nPenNo,                 //要设置的笔号(0-255)
            char[] sPenName,            //笔名字，默认default
            ref int clr,                //笔颜色
            ref bool bDisableMark,      //是否使能笔号，true关闭笔号
            ref bool bUseDefParam,      //是否使用默认值
            ref int nMarkLoop,          //加工次数
            ref double dMarkSpeed,      //标刻次数mm/s
            ref double dPowerRatio,     //功率百分比(0-100%)	
            ref double dCurrent,        //电流A
            ref int nFreq,              //频率HZ
            ref double nQPulseWidth,    //Q脉冲宽度us	
            ref int nStartTC,           //开始延时us
            ref int nLaserOffTC,        //激光关闭延时us 
            ref int nEndTC,             //结束延时us
            ref int nPolyTC,            //拐角延时us   	
            ref double dJumpSpeed,      //跳转速度mm/s
            ref int nMinJumpDelayTCUs,  //最小跳转延时us
            ref int nMaxJumpDelayTCUs,  //最大跳转延时us
            ref double dJumpLengthLimit,//跳转长的极限
            ref double dPointTime,      //打点时间 ms
            ref bool nSpiContinueMode,  //SPI连续模式
            ref int nSpiWave,           //SPI波形选择
            ref int nYagMarkMode,       //YAG优化填充模式
            ref bool bPulsePointMode,   //脉冲点模式
            ref int nPulseNum,          //脉冲点数
            ref bool bEnableACCMode,    //使能加速模式
            ref double dEndComp,        //末点补偿
            ref double dAccDist,        //加速距离
            ref double dBreakAngle,     //中断角度
            ref bool bWobbleMode,       //抖动模式						 
            ref double bWobbleDiameter, //抖动直径 
            ref double bWobbleDist);    //抖动间距

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetPenParam")]
        protected static extern LmcErrCode LMC1_SETPENPARAM(
            int nPenNo,                 //要设置的笔号(0-255)					 
            int nMarkLoop,              //加工次数
            double dMarkSpeed,          //标刻次数mm/s
            double dPowerRatio,         //功率百分比(0-100%)	
            double dCurrent,            //电流A
            int nFreq,                  //频率HZ
            double nQPulseWidth,        //Q脉冲宽度us	
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
            double dFlySpeed);          //流水线速度s

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetPenParam2")]
        protected static extern LmcErrCode LMC1_SETPENPARAM2(
            int nPenNo,             //要设置的笔号(0-255)					 
            int nMarkLoop,          //加工次数
            double dMarkSpeed,      //标刻次数mm/s
            double dPowerRatio,     //功率百分比(0-100%)	
            double dCurrent,        //电流A
            int nFreq,              //频率HZ
            double nQPulseWidth,    //Q脉冲宽度us	
            int nStartTC,           //开始延时us
            int nLaserOffTC,        //激光关闭延时us 
            int nEndTC,             //结束延时us
            int nPolyTC,            //拐角延时us   	
            double dJumpSpeed,      //跳转速度mm/s
            int nJumpPosTC,         //跳转位置延时us
            int nJumpDistTC,        //跳转距离延时us	
            double dPointTime,      //打点时间
            int nSpiWave,           //SPI波形选择	
            bool bWobbleMode,       //抖动模式						 
            double bWobbleDiameter, //抖动直径 
            double bWobbleDist);    //抖动间距

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetPenParam4")]
        protected static extern LmcErrCode LMC1_SETPENPARAM4(
            int nPenNo,             //要设置的笔号(0-255)
            string sPenName,        //笔名字，默认default
            int clr,                //笔颜色
            bool bDisableMark,      //是否使能笔号，true关闭笔号
            bool bUseDefParam,      //是否使用默认值
            int nMarkLoop,          //加工次数
            double dMarkSpeed,      //标刻次数mm/s
            double dPowerRatio,     //功率百分比(0-100%)	
            double dCurrent,        //电流A
            int nFreq,              //频率HZ
            double nQPulseWidth,    //Q脉冲宽度us	
            int nStartTC,           //开始延时us
            int nLaserOffTC,        //激光关闭延时us 
            int nEndTC,             //结束延时us
            int nPolyTC,            //拐角延时us   	
            double dJumpSpeed,      //跳转速度mm/s
            int nMinJumpDelayTCUs,  //最小跳转延时us
            int nMaxJumpDelayTCUs,  //最大跳转延时us
            double dJumpLengthLimit,//跳转长的极限
            double dPointTime,      //打点时间 ms
            bool nSpiContinueMode,  //SPI连续模式
            int nSpiWave,           //SPI波形选择
            int nYagMarkMode,       //YAG优化填充模式
            bool bPulsePointMode,   //脉冲点模式
            int nPulseNum,          //脉冲点数
            bool bEnableACCMode,    //使能加速模式
            double dEndComp,        //末点补偿
            double dAccDist,        //加速距离
            double dBreakAngle,     //中断角度
            bool bWobbleMode,       //抖动模式						 
            double bWobbleDiameter, //抖动直径 
            double bWobbleDist);    //抖动间距

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ SetPenDisableState")]
        protected static extern LmcErrCode LMC1_SETPENDISABLESTATE(int nPenNo, bool bDisableMark);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ GetPenDisableState")]
        protected static extern LmcErrCode LMC1_GETPENDISABLESTATE(int nPenNo, ref bool bDisableMark);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ GetPenNumberFromName")]
        protected static extern int LMC1_GETPENNUMBERFROMNAME(string sPenName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ GetPenNumberFromEnt")]
        protected static extern int LMC1_GETPENNUMBERFROMENT(string sEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ SetEntAllChildPen")]
        protected static extern LmcErrCode LMC1_SETENTALLCHILDPEN(string sEntName, int nPenNo);

        #endregion

        #region 填充

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetHatchParam")]
        protected static extern LmcErrCode LMC1_SETHATCHPARAM(
            bool bEnableContour,            //使能轮廓本身
            int bEnableHatch1,              //使能填充1
            int nPenNo1,                    //填充笔
            int NHatchAttrib1,              //填充属性
            double dHatchEdgeDist1,         //填充线边距
            double dHatchLineDist1,         //填充线间距
            double dHatchStartOffset1,      //填充线起始偏移距离
            double dHatchEndOffset1,        //填充线结束偏移距离
            double dHatchAngle1,            //填充线角度（弧度值）
            int bEnableHatch2,              //使能填充2
            int nPenNo2,                    //填充笔
            int nHatchAttrib2,              //填充属性
            double dHatchEdgeDist2,         //填充线边距
            double dHatchLineDist2,         //填充线间距
            double dHatchStartOffset2,      //填充线起始偏移距离
            double dHatchEndOffset2,        //填充线结束偏移距离
            double dHatchAngle2);           //填充线角度（弧度值）

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetHatchParam2")]
        protected static extern LmcErrCode LMC1_SETHATCHPARAM2(
            bool bEnableContour,            //使能轮廓本身
            int nParamIndex,                //填充参数序号值为1，2，3
            int bEnableHatch,               //使能填充
            int nPenNo,                     //填充参数笔号
            int nHatchType,                 //填充类型 0单向 1双向 2回形 3弓形 4弓形不反向
            bool bHatchAllCalc,             //是否全部对象作为整体一起计算
            bool bHatchEdge,                //绕边一次
            bool bHatchAverageLine,         //自动平均分布线
            double dHatchAngle,             //填充线角度
            double dHatchLineDist,          //填充线间距
            double dHatchEdgeDist,          //填充线边距
            double dHatchStartOffset,       //填充线起始偏移距离
            double dHatchEndOffset,         //填充线结束偏移距离
            double dHatchLineReduction,     //直接缩进
            double dHatchLoopDist,          //环间距
            int nEdgeLoop,                  //环数
            bool nHatchLoopRev,             //环形反转
            double dHatchAutoRotate,        //是否自动旋转角度
            double dHatchRotateAngle);      //自动旋转角度

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetHatchEntParam")]
        protected static extern LmcErrCode LMC1_SETHATCHENTPARAM(
            string pHatchEntParam,          //填充对象名称
            bool bEnableContour,            //使能轮廓本身
            int nParamIndex,                //填充参数序号值为1，2，3
            int bEnableHatch,               //使能填充
            int nPenNo,                     //填充参数笔号
            int nHatchType,                 //填充类型 0单向 1双向 2回形 3弓形 4弓形不反向
            bool bHatchAllCalc,             //是否全部对象作为整体一起计算
            bool bHatchEdge,                //绕边一次
            bool bHatchAverageLine,         //自动平均分布线
            double dHatchAngle,             //填充线角度
            double dHatchLineDist,          //填充线间距
            double dHatchEdgeDist,          //填充线边距
            double dHatchStartOffset,       //填充线起始偏移距离
            double dHatchEndOffset,         //填充线结束偏移距离
            double dHatchLineReduction,     //直接缩进
            double dHatchLoopDist,          //环间距
            int nEdgeLoop,                  //环数
            bool nHatchLoopRev,             //环形反转
            double dHatchAutoRotate,        //是否自动旋转角度
            double dHatchRotateAngle);      //自动旋转角度

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetHatchEntParam2")]
        protected static extern LmcErrCode LMC1_SETHATCHENTPARAM2(
            string pHatchEntParam,          //填充对象名称
            bool bEnableContour,            //使能轮廓本身
            int nParamIndex,                //填充参数序号值为1，2，3
            int bEnableHatch,               //使能填充
            bool bContourFirst,             //轮廓优先标刻
            int nPenNo,                     //填充参数笔号
            int nHatchType,                 //填充类型 0单向 1双向 2回形 3弓形 4弓形不反向
            bool bHatchAllCalc,             //是否全部对象作为整体一起计算
            bool bHatchEdge,                //绕边一次
            bool bHatchAverageLine,         //自动平均分布线
            double dHatchAngle,             //填充线角度
            double dHatchLineDist,          //填充线间距
            double dHatchEdgeDist,          //填充线边距
            double dHatchStartOffset,       //填充线起始偏移距离
            double dHatchEndOffset,         //填充线结束偏移距离
            double dHatchLineReduction,     //直接缩进
            double dHatchLoopDist,          //环间距
            int nEdgeLoop,                  //环数
            bool nHatchLoopRev,             //环形反转
            double dHatchAutoRotate,        //是否自动旋转角度
            double dHatchRotateAngle,       //自动旋转角度
            double bHatchCrossMode,         //交叉填充
            int dCycCount);                 //数目

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetHatchEntParam")]
        protected static extern LmcErrCode LMC1_GETHATCHENTPARAM(
            string pHatchEntParam,              //填充对象名称
            ref bool bEnableContour,            //使能轮廓本身
            ref int nParamIndex,                //填充参数序号值为1，2，3
            ref int bEnableHatch,               //使能填充
            ref int nPenNo,                     //填充参数笔号
            ref int nHatchType,                 //填充类型 0单向 1双向 2回形 3弓形 4弓形不反向
            ref bool bHatchAllCalc,             //是否全部对象作为整体一起计算
            ref bool bHatchEdge,                //绕边一次
            ref bool bHatchAverageLine,         //自动平均分布线
            ref double dHatchAngle,             //填充线角度
            ref double dHatchLineDist,          //填充线间距
            ref double dHatchEdgeDist,          //填充线边距
            ref double dHatchStartOffset,       //填充线起始偏移距离
            ref double dHatchEndOffset,         //填充线结束偏移距离
            ref double dHatchLineReduction,     //直接缩进
            ref double dHatchLoopDist,          //环间距
            ref int nEdgeLoop,                  //环数
            ref bool nHatchLoopRev,             //环形反转
            ref double dHatchAutoRotate,        //是否自动旋转角度
            ref double dHatchRotateAngle);      //自动旋转角度

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetHatchEntParam2")]
        protected static extern LmcErrCode LMC1_GETHATCHENTPARAM2(
            string pHatchEntParam,              //填充对象名称
            ref bool bEnableContour,            //使能轮廓本身
            ref int nParamIndex,                //填充参数序号值为1，2，3
            ref int bEnableHatch,               //使能填充
            ref bool bContourFirst,             //轮廓优先标刻
            ref int nPenNo,                     //填充参数笔号
            ref int nHatchType,                 //填充类型 0单向 1双向 2回形 3弓形 4弓形不反向
            ref bool bHatchAllCalc,             //是否全部对象作为整体一起计算
            ref bool bHatchEdge,                //绕边一次
            ref bool bHatchAverageLine,         //自动平均分布线
            ref double dHatchAngle,             //填充线角度
            ref double dHatchLineDist,          //填充线间距
            ref double dHatchEdgeDist,          //填充线边距
            ref double dHatchStartOffset,       //填充线起始偏移距离
            ref double dHatchEndOffset,         //填充线结束偏移距离
            ref double dHatchLineReduction,     //直接缩进
            ref double dHatchLoopDist,          //环间距
            ref int nEdgeLoop,                  //环数
            ref bool nHatchLoopRev,             //环形反转
            ref double dHatchAutoRotate,        //是否自动旋转角度
            ref double dHatchRotateAngle,       //自动旋转角度
            ref double bHatchCrossMode,         //交叉填充
            ref int dCycCount);                 //数目

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_HatchEnt")]
        protected static extern LmcErrCode LMC1_HATCHENT(string pEntName, string pEntNameNew);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_UnHatchEnt")]
        protected static extern LmcErrCode LMC1_UNHATCHENT(string pHatchEntName);

        #endregion

        #region 添加删除对象

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ClearEntLib")]
        protected static extern LmcErrCode LMC1_CLEARENTLIB();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_DeleteEntLib")]
        protected static extern LmcErrCode LMC1_DELETEENTLIB(string pEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AddTextToLib")]
        protected static extern LmcErrCode LMC1_ADDTEXTTOLIB(string pStr, string pEntName, double dPosX, double dPosY, double dPosZ, int nAlign, double dTextRotateAngle, int nPenNo, bool bHatchText);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AddCurveToLib")]
        protected static extern LmcErrCode LMC1_ADDCURVETOLIB(double[][] ptBuf, int ptNum, string pEntName, int nPenNo, int bHatch);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AddPointToLib")]
        protected static extern LmcErrCode LMC1_ADDPOINTTOLIB(double[][] ptBuf, int ptNum, string pEntName, int nPenNo);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AddDelayToLib")]
        protected static extern LmcErrCode LMC1_ADDDELAYTOLIB(double dDelayMs);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AddWritePortToLib")]
        protected static extern LmcErrCode LMC1_ADDWRITEPORTTOLIB(int nOutPutBit, bool bHigh, bool bPulse, double dPulseTimeMs);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AddFileToLib")]
        protected static extern LmcErrCode LMC1_ADDFILETOLIB(string pFileName, string pEntName, double dPosX, double dPosY, double dPosZ, int nAlign, double dRatio, int nPenNo, bool bHatchFile);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AddBarCodeToLib")]
        protected static extern LmcErrCode LMC1_ADDBARCODETOLIB(
            string pStr,
            string pEntName,
            double dPosX,
            double dPosY,
            double dPosZ,
            int nAlign,
            int nPenNo,
            int bHatchText,
            int nBarcodeType,
            IntPtr wBarCodeAttrib,
            double dHeight,
            double dNarrowWidth,
            double[] dBarWidthScale,
            double[] dSpaceWidthScale,
            double dMidCharSpaceScale,
            double dQuietLeftScale,
            double dQuietMidScale,
            double dQuietRightScale,
            double dQuietTopScale,
            double dQuietBottomScale,
            int nRow,
            int nCol,
            int nCheckLevel,
            int nSizeMode,
            double dTextHeight,
            double dTextWidth,
            double dTextOffsetX,
            double dTextOffsetY,
            double dTextSpace,
            double dDiameter,
            string pTextFontName);

        #endregion

        #region 扩展轴

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Reset")]
        protected static extern LmcErrCode LMC1_RESET(bool bEnAxis0, bool bEnAxis1);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AxisCorrectOrigin")]
        protected static extern LmcErrCode LMC1_AXISCORRECTORIGION(int axis);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AxisMoveTo")]
        protected static extern LmcErrCode LMC1_AXISMOVETO(int axis, double GoalPos);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ AxisMoveToPulse")]
        protected static extern LmcErrCode LMC1_AXISMOVETOPULSE(int axis, int GoalPos);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetAxisCoor")]
        protected static extern double LMC1_GETAXISCOOR(int axis);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetAxisCoorPulse")]
        protected static extern double LMC1_GETAXISCOORPULSE(int axis);

        #endregion

    }
}
