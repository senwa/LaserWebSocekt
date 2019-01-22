using System;
using System.Runtime.InteropServices; 
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;

namespace MarkEzd
{
    #region ErrorCode
    public enum LmcErrCode
    {
        LMC1_ERR_SUCCESS = 0,
        LMC1_ERR_EZCADRUN = 1,
        LMC1_ERR_NOFINDCFGFILE = 2,
        LMC1_ERR_FAILEDOPEN = 3,
        LMC1_ERR_NODEVICE = 4,
        LMC1_ERR_HARDVER = 5,
        LMC1_ERR_DEVCFG = 6,
        LMC1_ERR_STOPSIGNAL = 7,
        LMC1_ERR_USERSTOP = 8,
        LMC1_ERR_UNKNOW = 9,
        LMC1_ERR_OUTTIME = 10,
        LMC1_ERR_NOINITIAL = 11,
        LMC1_ERR_READFILE = 12,
        LMC1_ERR_OWENWNDNULL = 13,
        LMC1_ERR_NOFINDFONT = 14,
        LMC1_ERR_PENNO = 15,
        LMC1_ERR_NOTTEXT = 16,
        LMC1_ERR_SAVEFILE = 17,
        LMC1_ERR_NOFINDENT = 18,
        LMC1_ERR_STATUE = 19,
        LMC1_ERR_PARAM = 20,
    }
    #endregion

    public class MarkEzd_Dll
    {
        #region ExportDll

        /// <summary>
        /// 初始化lmc1控制卡
        /// 输入参数: strEzCadPath  EzCad软件的执行路径
        /// bTestMode = TRUE 表示测试模式  bTestMode = FALSE 表示正常模式
        /// pOwenWnd      表示父窗口对象，如果需要实现停止打标，则系统会从此窗口截取消息
        /// </summary>
        /// <param name="strEzCadPath"></param>
        /// <param name="bTestMode"></param>
        /// <param name="hOwenWnd"></param>
        /// <returns></returns>
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Initial")]
        protected static extern LmcErrCode LMC1_INITIAL(string strEzCadPath, int bTestMode, IntPtr hOwenWnd);

        //
        // 关闭lmc1控制卡
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Close")]
        protected static extern LmcErrCode LMC1_CLOSE();

        //
        //载入ezd文件，并清除数据库所有对象
        //输入参数: strFileName  EzCad文件名称
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_LoadEzdFile")]
        protected static extern LmcErrCode LMC1_LOADEZDFILE(string strFileName);

        //
        // 出光
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_Mark")]
        protected static extern LmcErrCode LMC1_MARK(int nFly);

        //
        // 替换文本变量
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_ChangeTextByName")]
        protected static extern LmcErrCode LMC1_CHANGETEXTBYNAME(string strEntName, string strText);

        //
        // 读输入端口
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ReadPort")]
        protected static extern LmcErrCode LMC1_READPORT(ref int nData);

        //
        // 写输出端口
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_WritePort")]
        protected static extern LmcErrCode LMC1_WRITEPORT(int nData);

        /// <summary>
        /// 预览图片，该函数在C#中不能使用
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nBMPWIDTH"></param>
        /// <param name="nBMPHEIGHT"></param>
        /// <returns></returns>
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPrevBitmap")]
        protected static extern IntPtr LMC1_GETPREVBITMAP(IntPtr hwnd, int nBMPWIDTH, int nBMPHEIGHT);

        /// <summary>
        /// 预览图片，该函数可以在C#中使用
        /// </summary>
        /// <param name="nBMPWIDTH"></param>
        /// <param name="nBMPHEIGHT"></param>
        /// <returns></returns>
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPrevBitmap2")]
        protected static extern IntPtr LMC1_GETPREVBITMAP2(int nBMPWIDTH, int nBMPHEIGHT);

        //调用设置设备参数的对话框
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetDevCfg")]
        protected static extern LmcErrCode LMC1_SETDEVCFG();

        //
        // 获取参数
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_GetPenParam")]
        protected static extern LmcErrCode lmc1_GetPenParam(int nPenNo,//要设置的笔号(0-255)					 
                    ref int nMarkLoop,//加工次数
                        ref double dMarkSpeed,//标刻次数mm/s
                        ref double dPowerRatio,//功率百分比(0-100%)	
                        ref double dCurrent,//电流A
                        ref int nFreq,//频率HZ
                    ref double dQPulseWidth,//Q脉冲宽度us	
                        ref int nStartTC,//开始延时us
                        ref int nLaserOffTC,//激光关闭延时us 
                        ref int nEndTC,//结束延时us
                        ref int nPolyTC,//拐角延时us   //	
                        ref double dJumpSpeed, //跳转速度mm/s
                        ref int nJumpPosTC, //跳转位置延时us
                    ref int nJumpDistTC,//跳转距离延时us	
                        ref double dEndComp,//末点补偿mm
                        ref double dAccDist,//加速距离mm	
                        ref double dPointTime,//打点延时 ms						 
                    ref bool bPulsePointMode,//脉冲点模式 
                    ref int nPulseNum,//脉冲点数目
                        ref double dFlySpeed);//流水线速度

        //
        // 设置参数
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_SetPenParam")]
        protected static extern LmcErrCode lmc1_SetPenParam(int nPenNo,//要设置的笔号(0-255)					 
                        int nMarkLoop,//加工次数
                        double dMarkSpeed,//标刻次数mm/s
                        double dPowerRatio,//功率百分比(0-100%)	
                        double dCurrent,//电流A
                            int nFreq,//频率HZ
                        double dQPulseWidth,//Q脉冲宽度us	
                        int nStartTC,//开始延时us
                        int nLaserOffTC,//激光关闭延时us 
                            int nEndTC,//结束延时us
                        int nPolyTC,//拐角延时us   //	
                        double dJumpSpeed, //跳转速度mm/s
                        int nJumpPosTC, //跳转位置延时us
                        int nJumpDistTC,//跳转距离延时us	
                        double dEndComp,//末点补偿mm
                        double dAccDist,//加速距离mm	
                        double dPointTime,//打点延时 ms						 
                        bool bPulsePointMode,//脉冲点模式 
                        int nPulseNum,//脉冲点数目
                        double dFlySpeed);//流水线速度

        //
        // 平移
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_MoveEnt")]
        protected static extern LmcErrCode LMC1_MOVEENT(string pEntName, double dMovex, double dMovey);

        // 红光
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_RedLightMark")]
        protected static extern LmcErrCode LMC1_REDLIGHTMARK();

        /// <summary>
        /// 获取对象总个数
        /// </summary>
        /// <returns></returns>
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntityCount")]
        protected static extern int LMC1_GETENTITYCOUNT();

        //
        //得到指定序号的对象名称
        //输入参数: nEntityIndex 指定对象的序号(围: 0 － (lmc1_GetEntityCount()-1))
        //输出参数: szEntName 对象的名称
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntityName")]
        protected static extern LmcErrCode LMC1_GETENTITYNAME(int nEntityIndex, char[] strEntName);

        //
        // 停止出光
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_StopMark")]
        protected static extern LmcErrCode LMC1_STOPMARK();

        /// <summary>
        /// 轴移动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="GoalPos"></param>
        /// <returns></returns>
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AxisMoveTo")]
        protected static extern LmcErrCode lmc1_AxisMoveTo(int axis, double GoalPos);

        //
        //保存文本
        //输入参数: strFileName  EzCad文件名称
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SaveEntLibToFile")]
        protected static extern LmcErrCode lmc1_SaveEntLibToFile(string strFileName);

        //
        // 取得实体对象的位置
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntSize")]
        protected static extern LmcErrCode LMC1_GETENTSIZE(string strEntName, ref double dMinx, ref double dMiny, ref double dMaxx, ref double dMaxy, ref double dZ);

        //
        // 旋转
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_RotateEnt")]
        protected static extern LmcErrCode LMC1_ROTATEENT(string strEntName, double dCenx, double dCeny, double dAngle);

        //
        //得到指定序号的对象名称
        //输入参数: nEntityIndex 指定对象的序号(围: 0 － (lmc1_GetEntityCount()-1))
        //输出参数: szEntName 对象的名称
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_DeleteEnt")]
        protected static extern LmcErrCode lmc1_DeleteEnt(string strEntName);

        //
        // 读输出端口状态
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetOutPort")]
        protected static extern LmcErrCode LMC1_GETOUTPORT(ref int nData);


        //设置所有对象的先绕旋转中心旋转指定角度，然后平移指定距离
        //输入参数: dMoveX   为x平移的距离
        //          dMoveY   为y平移的距离
        //          dCenterX 旋转中心x坐标 
        //          dCenterY 旋转中心y坐标 
        //          dRotateAng 旋转角度(弧度值),
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetRotateMoveParam")]
        protected static extern void lmc1_SetRotateMoveParam(double dMoveX, double dMoveY,
            double dCenterX, double dCenterY,
            double dRotateAng);

        /// <summary>
        /// 释放指针
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        protected static extern bool DeleteObject(IntPtr hObject);

        //
        //得到指定序号的对象名称
        //输入参数: nEntityIndex 指定对象的序号(围: 0 － (lmc1_GetEntityCount()-1))
        //输出参数: szEntName 对象的名称
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetEntityName")]
        protected static extern LmcErrCode LMC1_SETENTITYNAME(int nEntityIndex, string strEntName);


        //根据对象名字获取笔号
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPenNumberFromEnt")]
        protected static extern int lmc1_GetPenNumberFromEnt(string strEntName);

        //根据对象名字获取笔号
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPenNumberFromName")]
        protected static extern int lmc1_GetPenNumberFromName(string strEntName);

        #endregion
    }
}
