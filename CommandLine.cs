using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.IO;

namespace StandaloneSDKDemo
{
    class CommandLine
    {
        SDKHelperV2 SDK = new SDKHelperV2();
        List<String> lbSysOutputInfo = new List<String>();
        
        public String YearMonth
        {
            get;
            set;
        }

        public List<String> getSystemOutput()
        {
            return lbSysOutputInfo;
        }
        public String getVersion()
        {
            string ver = null;
            this.SDK.axCZKEM1.GetSDKVersion(ref ver);

            return  ver;
        }

        public void CloseSDK()
        {
            this.SDK.sta_DisConnect();
        }
        private void getDeviceInfo()
        {
            string sFirmver = "";
            string sMac = "";
            string sPlatform = "";
            string sSN = "";
            string sProductTime = "";
            string sDeviceName = "";
            int iFPAlg = 0;
            int iFaceAlg = 0;
            string sProducter = "";

            this.SDK.sta_GetDeviceInfo(this.lbSysOutputInfo, out sFirmver, out sMac, out sPlatform, out sSN, out sProductTime,
                                    out sDeviceName, out iFPAlg, out iFaceAlg, out sProducter);
            
            Console.Out.WriteLine("sta_GetDeviceInfo");

            Console.Out.WriteLine(sFirmver);
            Console.Out.WriteLine(sMac);
            Console.Out.WriteLine(sSN);
            Console.Out.WriteLine(sPlatform);
            Console.Out.WriteLine(sDeviceName);
            Console.Out.WriteLine(iFPAlg.ToString().Trim());
            Console.Out.WriteLine(iFaceAlg.ToString().Trim());
            Console.Out.WriteLine(sProducter);
            Console.Out.WriteLine(sProductTime);

            Console.Out.WriteLine("END sta_GetDeviceInfo");
        }
        private void getCapacityInfo()
        {
            int adminCnt = 0;
            int userCount = 0;
            int fpCnt = 0;
            int recordCnt = 0;
            int pwdCnt = 0;
            int oplogCnt = 0;
            int faceCnt = 0;
            this.SDK.sta_GetCapacityInfo(this.lbSysOutputInfo, out adminCnt, out userCount, out fpCnt, out recordCnt, out pwdCnt, out oplogCnt, out faceCnt);

            Console.Out.WriteLine("getCapacityInfo");

            Console.Out.WriteLine(adminCnt.ToString());
            Console.Out.WriteLine(userCount.ToString());
            Console.Out.WriteLine(fpCnt.ToString());
            Console.Out.WriteLine(recordCnt.ToString());
            Console.Out.WriteLine(pwdCnt.ToString());
            Console.Out.WriteLine(oplogCnt.ToString());
            Console.Out.WriteLine(faceCnt.ToString());

            Console.Out.WriteLine("END getCapacityInfo");
        }
        public void connectDevice(String txtIP, String txtPort, String txtCommKey)
        {
            //txtIP = "192.168.1.222";
            //txtPort = "4370";
            //txtCommKey = "0";

            int ret = this.SDK.sta_ConnectTCP(this.lbSysOutputInfo, txtIP.Trim(), txtPort.Trim(), txtCommKey.Trim());

            if (this.SDK.GetConnectState())
            {
                this.SDK.sta_getBiometricType();
            }

            if (ret == 1)
            {
                
                getCapacityInfo();
                getDeviceInfo();

            }
            else if (ret == -2)
            {
                Console.Out.WriteLine("Error: Cannot connect to Device");
            }
        }
        private DataTable getDummyAttTable()
        {
            DataTable dt_period = new DataTable("dt_period");
            dt_period.Columns.Add("User ID");
            dt_period.Columns.Add("Verify Date");
            dt_period.Columns.Add("Verify Type");
            dt_period.Columns.Add("Verify State");
            dt_period.Columns.Add("WorkCode");
            return dt_period;
        }
        public DataTable readAllAttendantInOut()
        {
            DataTable dt_period = getDummyAttTable();

            //dr["User ID"] = sdwEnrollNumber;
            //dr["Verify Date"] = idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond;
            //dr["Verify Type"] = idwVerifyMode;
            //dr["Verify State"] = idwInOutMode;
            //dr["WorkCode"] = idwWorkcode;

            this.SDK.sta_readAttLog(this.lbSysOutputInfo, dt_period);
            return dt_period;
        }
        public void readAllAttendantInOut2CSV(String filename)
        {
            this.SDK.sta_readAttLog2CSV(this.lbSysOutputInfo, filename, this.YearMonth);
        }
        public void readAllUsers2CSV(String fileName)
        {
            if (this.SDK == null) return;

            if (this.SDK.GetConnectState())
            {

                this.SDK.sta_getEmployees2CSV(fileName);

                this.lbSysOutputInfo.Add("Download successfully");
            }
        }
        public DataTable readAttendantInOut(string fromTime, string toTime)
        {
            // fromTime = yyyy-MM-dd hh:mm
            // toTime   = yyyy-MM-dd hh:mm
            DataTable dt_period = getDummyAttTable();

            this.SDK.sta_readLogByPeriod(this.lbSysOutputInfo, dt_period, fromTime, toTime);
            return dt_period;
        }

        public void deleteAllAttendantInOut()
        {
            this.SDK.sta_DeleteAttLog(this.lbSysOutputInfo);
        }
        public void deleteAllAttendantInOut(string fromTime, string toTime)
        {
            this.SDK.sta_DeleteAttLogByPeriod(this.lbSysOutputInfo, fromTime, toTime);
        }

        internal void deleteEmployeeId(string emplId)
        {
            int id = 0;
            if (int.TryParse(emplId, out id))
            {
                //this.SDK.sta_DeleteEnrollData(this.lbSysOutputInfo, emplId, "12");// Delete user 
            }
        }
        internal String getFP(String emplID, int idwFingerIndex, String fileName)
        {
            int iFlag = 0;
            int iFPTmpLength = 0;
            String sFPTmpData = "";

            this.SDK.axCZKEM1.GetUserTmpExStr(this.SDK.GetMachineNumber(), emplID, idwFingerIndex, out iFlag, out sFPTmpData, out iFPTmpLength);
            using (StreamWriter streamwriter = File.CreateText(fileName))
            {
                streamwriter.Write(sFPTmpData);
            }
            return sFPTmpData;
        }
        internal bool setFP(String emplID, Int32 idwFingerIndex, String sFPTmpData, Int32 iFlag)
        {
            return this.SDK.axCZKEM1.SetUserTmpExStr(this.SDK.GetMachineNumber(), emplID, idwFingerIndex, iFlag, sFPTmpData);
        }
        internal String getAllText(String fileName)
        {
            String s = File.ReadAllText(fileName);
            return s;
        }
    }
}
