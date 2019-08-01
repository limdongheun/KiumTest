/************************************************************
  샘플버전 : 1.0.0.0 ( 2015.01.23 )
  샘플제작 : (주)에스비씨엔 / sbcn.co.kr/ ZooATS.com
  샘플환경 : Visual Studio 2013 / C# 5.0
  샘플문의 : support@zooats.com / john@sbcn.co.kr
  전    화 : 02-719-5500 / 070-7777-6555
************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KiwoomCode;
using System.IO;

namespace KOASampleCS
{
    public partial class Form1 : Form
    {
        public struct TRADE
        {
            public string[] sCode;          //종목코드
            public string[] sName;          //종목이름
            public int[] nClosePrice;      //어제 종가
            public int[] nNowPrice;      //현재 가격
            public int[] nState;            //상태 0-매수대기, 1-매수접수, 2-매수확인, 3-매수완료, 4-매도접수, 5-매도확인, 6-매도완료
            public string[] sOrderNo;       //주문번호
            public int[] nOrderQty;             //매수수량
            public int[] nBuyQty;             //매수수량
            public int[] nBuyPrice;           //매수단가
            public int[] nSellQty;             //매도수량
            public int[] nSellPrice;           //매도단가
            public int[] nSellCount;           //매도횟수
            public int[] nBuyTime;          //매수주문시간
            public int[] nSellTime;          //매도주문시간
            public int[] nHighPrice;        //최고금액
            public int[] nHighTime;        //최고시간
            public int[] nLowPrice;        //최저금액
            public int[] nLowTime;        //최저시간
            public int[] nStandardPrice;        //기준금액
            public int[] nStandardTime;        //기준시간
            public bool[] bEndSell;         //마감구매종목
            public int[] n3HourStartPrice;        //3시 초반금액
            public int[] n3HourLastPrice;        //3시 마감금액
            public bool[] bSellSignal;      //매수신호
            public int[] nPivot;            //피봇2차저항
            public int[] nPivotBuyPrice;            //피봇2차저항 돌파 매수금액
            public int[] nCheckTime;            //피봇돌파 후 체크 시간

            public bool[] bHighPriceCheck;      //고가 체크
            public int[] nAverageStatus;     //이평선 상태
            public int[] nUpAverageHigh1;     //상승 이평선 최고가1
            public int[] nUpAverageHigh2;     //상승 이평선 최고가2
            public int[] nUpAverageEnd1;     //상승 이평선 종가1
            public int[] nUpAverageEnd2;     //상승 이평선 종가2
            public int[] nDownAverageHigh1;     //하강 이평선 최고가1
            public int[] nDownAverageHigh2;     //하강 이평선 최고가2
            public int[] nDownAverageEnd1;     //하강 이평선 종가1
            public int[] nDownAverageEnd2;     //하강 이평선 종가2
            public int[] n5MinuteAverage;   //5분 이평선
            public int[] n10MinuteAverage;   //10분 이평선
            public int[,] n5MinutePrice;     //5분 금액
            public int[,] n10MinutePrice;     //10분 금액

            public string[,] sMType;      //5분봉 타입
            public int[,] nMStartPrice;      //분당 시작가
            public int[,] nMEndPrice;      //분당 종가
            public int[,] nMHighPrice;      //분당 최고가
            public int[,] nMLowPrice;      //분당 최저가
            public int[,] nMTime;           //분당 시간
            public int[,] nMHighTime;      //분당 최고가 시간
            public int[,] nMLowTime;      //분당 최저가 시간
            public int[] nMCount;           //현재 저장 카운트
            public long[,] lMTradVol;        //분당 거래량
            public long[,] lMTradVolAll;        //전체 거래량
        };

        public struct PRICE
        {

        }

        TRADE stTradeData;

        public bool m_bLiveCheckThread = false;
        public bool m_bTradeDataCheckThread = false;
        public int m_nCondNumber = 0;// Int32.Parse(spCon[0]);    // 조건번호
        public string m_spCondName = "";// spCon[1];               // 조건식 이름
        public int m_nTradeCount = 0;
        public string m_sNowPrice = "";
        public int m_nAllAsset = 0;
        public bool m_bFirst = true;
        public bool m_bStartSell = false;
        public int m_nCloseSellCount = 0;
        public int m_nStartSellCount = 0;
        public int m_nAverageHigh = 0;
        public bool m_bSale = true;
        public bool m_bNextDayChcek = true;
        public bool m_bNextMinChcek = true;

        public struct ConditionList
        {
            public string strConditionName;
            public int nIndex;
        }

        private int _scrNum = 5000;
        private string _strRealConScrNum = "0000";
        private string _strRealConName = "0000";
        private int _nIndex = 0;

        private bool _bRealTrade = false;

        // 화면번호 생산
        private string GetScrNum()
        {
            if (_scrNum < 9999)
                _scrNum++;
            else
                _scrNum = 5000;

            return _scrNum.ToString();
        }

        // 실시간 연결 종료
        private void DisconnectAllRealData()
        {
            for( int i = _scrNum; i > 5000; i-- )
            {
                axKHOpenAPI.DisconnectRealData(i.ToString());
            }

            _scrNum = 5000;
        }

        public Form1()
        {
            InitializeComponent();

            stTradeData.sCode = new string[200];
            stTradeData.sName = new string[200];
            stTradeData.nClosePrice = new int[200];
            stTradeData.nNowPrice = new int[200];
            stTradeData.nState = new int[200];
            stTradeData.sOrderNo = new string[200];
            stTradeData.nOrderQty = new int[200];
            stTradeData.nBuyQty = new int[200];
            stTradeData.nBuyPrice = new int[200];
            stTradeData.nSellQty = new int[200];
            stTradeData.nSellPrice = new int[200];
            stTradeData.nSellCount = new int[200];
            stTradeData.nBuyTime = new int[200];
            stTradeData.nSellTime = new int[200];
            stTradeData.nHighPrice = new int[200];
            stTradeData.nHighTime = new int[200];
            stTradeData.nLowPrice = new int[200];
            stTradeData.nLowTime = new int[200];
            stTradeData.nStandardPrice = new int[200];
            stTradeData.nStandardTime = new int[200];
            stTradeData.bEndSell = new bool[200];
            stTradeData.n3HourStartPrice = new int[200];
            stTradeData.n3HourLastPrice = new int[200];
            stTradeData.bSellSignal = new bool[200];
            stTradeData.nPivot = new int[200];
            stTradeData.nPivotBuyPrice = new int[200];
            stTradeData.nCheckTime = new int[200];

            stTradeData.bHighPriceCheck = new bool[200];
            stTradeData.nAverageStatus = new int[200];
            stTradeData.nUpAverageHigh1 = new int[200];
            stTradeData.nUpAverageHigh2 = new int[200];
            stTradeData.nUpAverageEnd1 = new int[200];
            stTradeData.nUpAverageEnd2 = new int[200];
            stTradeData.nDownAverageHigh1 = new int[200];
            stTradeData.nDownAverageHigh2 = new int[200];
            stTradeData.nDownAverageEnd1 = new int[200];
            stTradeData.nDownAverageEnd2 = new int[200];

            stTradeData.n5MinuteAverage = new int[200];
            stTradeData.n10MinuteAverage = new int[200];
            stTradeData.n5MinutePrice = new int[200, 5];
            stTradeData.n10MinutePrice = new int[200, 10];

            stTradeData.sMType = new string[200, 500];
            stTradeData.nMStartPrice = new int[200,500];
            stTradeData.nMEndPrice = new int[200, 500];
            stTradeData.nMHighPrice = new int[200, 500];
            stTradeData.nMLowPrice = new int[200, 500];
            stTradeData.nMTime = new int[200, 500];
            stTradeData.nMHighTime = new int[200, 500];
            stTradeData.nMLowTime = new int[200, 500];
            stTradeData.nMCount = new int[200];
            stTradeData.lMTradVol = new long[200, 500];
            stTradeData.lMTradVolAll = new long[200, 500];

            for (int i = 0; i < 200; i++)
            {
                stTradeData.sCode[i] = "";
                stTradeData.sName[i] = "";
                stTradeData.nClosePrice[i] = 0;
                stTradeData.nNowPrice[i] = 0;
                stTradeData.nState[i] = 0;
                stTradeData.sOrderNo[i] = "";
                stTradeData.nOrderQty[i] = 0;
                stTradeData.nBuyQty[i] = 0;
                stTradeData.nBuyPrice[i] = 0;
                stTradeData.nSellQty[i] = 0;
                stTradeData.nSellPrice[i] = 0;
                stTradeData.nSellCount[i] = 0;
                stTradeData.nBuyTime[i] = 0;
                stTradeData.nSellTime[i] = 0;
                stTradeData.nHighPrice[i] = 0;
                stTradeData.nHighTime[i] = 0;
                stTradeData.nStandardPrice[i] = 0;
                stTradeData.nStandardTime[i] = 0;
                stTradeData.bEndSell[i] = false;
                stTradeData.n3HourStartPrice[i] = 0;
                stTradeData.n3HourLastPrice[i] = 0;
                stTradeData.bSellSignal[i] = false;
                stTradeData.nPivot[i] = 0;
                stTradeData.nPivotBuyPrice[i] = 0;
                stTradeData.nCheckTime[i] = 0;

                stTradeData.bHighPriceCheck[i] = false;
                stTradeData.nAverageStatus[i] = 0;
                stTradeData.nUpAverageHigh1[i] = 0;
                stTradeData.nUpAverageHigh2[i] = 0;
                stTradeData.nUpAverageEnd1[i] = 0;
                stTradeData.nUpAverageEnd2[i] = 0;
                stTradeData.nDownAverageHigh1[i] = 0;
                stTradeData.nDownAverageHigh2[i] = 0;
                stTradeData.nDownAverageEnd1[i] = 0;
                stTradeData.nDownAverageEnd2[i] = 0;

                stTradeData.n5MinuteAverage[i] = 0;
                stTradeData.n10MinuteAverage[i] = 0;

                stTradeData.nMCount[i] = 0;

                for (int j = 0; j < 500; j++)
                {
                    stTradeData.sMType[i, j] = "";
                    stTradeData.nMStartPrice[i,j] = 0;
                    stTradeData.nMEndPrice[i, j] = 0;
                    stTradeData.nMHighPrice[i, j] = 0;
                    stTradeData.nMLowPrice[i, j] = 0;
                    stTradeData.nMTime[i, j] = 0;
                    stTradeData.nMHighTime[i, j] = 0;
                    stTradeData.nMLowTime[i, j] = 0;
                    stTradeData.lMTradVol[i, j] = 0;
                    stTradeData.lMTradVolAll[i, j] = 0;
                }

                for(int j = 0; j < 10; j++)
                {
                    if(j <5)
                    {
                        stTradeData.n5MinutePrice[i, j] = 0;
                    }
                    
                    stTradeData.n10MinutePrice[i, j] = 0;
                }
            }

            m_bTradeDataCheckThread = true;
            //System.Threading.Thread TradeThread = new System.Threading.Thread(new System.Threading.ThreadStart(TradeDataCheck1));
            //TradeThread.Start();

        }

        public void CheckLiveStock()
        {
            try
            {
                while(true)
                {
                    string hour = System.DateTime.Now.ToString("hh");

                    if (m_bLiveCheckThread == false || hour == "10")
                        return;

                    //string[] spCon = cbo조건식.Text.Split('^');
                    int nCondNumber = m_nCondNumber;// Int32.Parse(spCon[0]);    // 조건번호
                    string spCondName = m_spCondName;// spCon[1];               // 조건식 이름
                    int lRet = axKHOpenAPI.SendCondition(GetScrNum(), spCondName, nCondNumber, 0);

                    if (lRet == 1)
                    {
                        //Logger(Log.일반, "조건식 일반 조회 실행이 성공 되었습니다");
                    }
                    else
                    {
                        //Logger(Log.에러, "조건식 일반 조회 실행이 실패 하였습니다");
                    }

                    System.Threading.Thread.Sleep(60000);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddTradeList(string CodeList, int state = 0, int buyQty = 0, int buyPrice = 0)
        {
            string[] codes = CodeList.Split(';');
            int count = codes.Length;
            if (count > 1)
                count--;

            for (int i = 0; i < count; i++)
            {
                string sLastPrice = axKHOpenAPI.GetMasterLastPrice(codes[i]);

                if(sLastPrice != "")
                {
                    if (Convert.ToInt64(sLastPrice) < 20000)
                    {
                        int nSavePoint = 1000;
                        bool bSave = true;

                        for (int j = 0; j < 200; j++)
                        {
                            if (stTradeData.sCode[j] == "" && nSavePoint == 1000)
                            {
                                nSavePoint = j;
                            }

                            if (stTradeData.sCode[j] == codes[i])
                            {
                                bSave = false;
                                break;
                            }
                        }

                        if (bSave == true)
                        {
                            stTradeData.sCode[nSavePoint] = codes[i];
                            stTradeData.sName[nSavePoint] = axKHOpenAPI.GetMasterCodeName(codes[i]);
                            stTradeData.nClosePrice[nSavePoint] = Convert.ToInt32(sLastPrice);
                            stTradeData.nNowPrice[nSavePoint] = 0;
                            stTradeData.nState[nSavePoint] = state;
                            stTradeData.sOrderNo[nSavePoint] = "";
                            stTradeData.nBuyQty[nSavePoint] = buyQty;

                            if(state == 4)
                            {
                                stTradeData.bEndSell[nSavePoint] = true;
                                stTradeData.nState[nSavePoint] = 3;
                                stTradeData.nBuyPrice[nSavePoint] = buyPrice;
                                stTradeData.nBuyTime[nSavePoint] = 900;
                            }
                            else
                                stTradeData.nBuyPrice[nSavePoint] = 0;

                            stTradeData.nSellQty[nSavePoint] = 0;
                            stTradeData.nSellPrice[nSavePoint] = 0;
                            stTradeData.nBuyTime[nSavePoint] = 0;

                            LogManager.WriteLine("종목 :\t" + stTradeData.sCode[nSavePoint] + "\t" + stTradeData.sName[nSavePoint]);

                            string sFirst = "1";
                            if(m_bFirst == true)
                            {
                                m_bFirst = false;
                                sFirst = "0";
                            }

                            axKHOpenAPI.SetRealReg(GetScrNum(),              // 화면번호
                                            codes[i],    // 종콕코드 리스트
                                            "9001;10",  // FID번호
                                            sFirst);       // 0 : 마지막에 등록한 종목만 실시간
                        }
                    } 
                }
            }
        }

        public void ChangeTradeList(string Name, int nState, string sOrderNo, int nBuyQty, int nBuyPrice, int nSellQty, int nSellPrice)
        {
            int nHour = Convert.ToInt32(System.DateTime.Now.ToString("HH"));
            int nMinute = Convert.ToInt32(System.DateTime.Now.ToString("mm"));
            int nSecond = Convert.ToInt32(System.DateTime.Now.ToString("ss"));
            int nNowTime = nHour * 100 + nMinute;

            for (int i = 0; i < 200; i++)
            {
                if (stTradeData.sName[i] == Name)
                {
                    //stTradeData.nState[i] = nState;
                    stTradeData.sOrderNo[i] = sOrderNo;

                    LogManager.WriteLine("매매완료 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + sOrderNo + "\t" + nState.ToString());

                    if (nState <= 3)
                    {
                        if (stTradeData.nState[i] == 7 && nState == 3)
                        {
                            stTradeData.nBuyQty[i] += nBuyQty;
                            stTradeData.nBuyPrice[i] = nBuyPrice;
                            stTradeData.nBuyTime[i] = nNowTime;

                            LogManager.WriteLine("매수완료 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + stTradeData.nBuyQty[i].ToString() + "/" + stTradeData.nOrderQty[i].ToString() + " " + stTradeData.nBuyPrice[i].ToString());
                        }
                        else if (stTradeData.nBuyQty[i] > 0 && stTradeData.nBuyPrice[i] > 0 && nBuyPrice > 0 && nBuyQty > 0)
                        {
                            stTradeData.nState[i] = 3;

                            int nOldPrice = stTradeData.nBuyPrice[i] * stTradeData.nBuyQty[i];
                            int nNewPrice = nBuyPrice * nBuyQty;

                            LogManager.WriteLine("평균 :\t" + stTradeData.nBuyPrice[i] + "\t" + stTradeData.nBuyQty[i] + "\t" + nBuyPrice + "\t" + nBuyQty);

                            stTradeData.nBuyQty[i] += nBuyQty;
                            stTradeData.nBuyPrice[i] = (nOldPrice + nNewPrice) / stTradeData.nBuyQty[i];
                            stTradeData.nBuyTime[i] = nNowTime;
                        }
                        else if (stTradeData.nState[i] == 3)
                        {
                            stTradeData.nBuyQty[i] = nBuyQty;
                            stTradeData.nBuyPrice[i] = nBuyPrice;
                            stTradeData.nBuyTime[i] = nNowTime;
                        }

                        //if (stTradeData.nOrderQty[i] != stTradeData.nBuyQty[i])
                        //    stTradeData.nState[i] = 1;
                    }
                    else if (nState > 3)
                    {
                        if (stTradeData.nState[i] == 8 && nState == 6)
                        {
                            stTradeData.nSellQty[i] += nSellQty;
                            stTradeData.nSellPrice[i] = nSellPrice;

                            LogManager.WriteLine("매도완료 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + stTradeData.nSellQty[i].ToString() + "/" + stTradeData.nOrderQty[i].ToString() + " " + stTradeData.nSellPrice[i].ToString());

                            if (stTradeData.nSellQty[i] == stTradeData.nOrderQty[i])
                            {
                                stTradeData.nState[i] = 6;

                                stTradeData.bSellSignal[i] = false;
                                stTradeData.nBuyQty[i] = 0;
                                stTradeData.nBuyPrice[i] = 0;
                                stTradeData.nBuyTime[i] = 0;
                                stTradeData.nSellQty[i] = 0;
                                stTradeData.nSellPrice[i] = 0;
                                stTradeData.nSellTime[i] = nNowTime;
                                stTradeData.nSellCount[i]++;

                                if(stTradeData.nSellCount[i] == 2)
                                {
                                    stTradeData.nState[i] = 9;
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }

        public void TradeDataCheck1()
        {
            try
            {
                while (true)
                {
                    if (m_bTradeDataCheckThread == false)
                        return;

                    int nHour = Convert.ToInt32(System.DateTime.Now.ToString("HH"));
                    int nMinute = Convert.ToInt32(System.DateTime.Now.ToString("mm"));
                    int nSecond = Convert.ToInt32(System.DateTime.Now.ToString("ss"));
                    int nNowTime = nHour * 100 + nMinute;

                    int nTimeCount = (nHour - 9) * 60 + nMinute;

                    for (int i = 0; i < 200; i++)
                    {
                        if(stTradeData.sCode[i] != "")
                        {
                            if (stTradeData.nState[i] == 0 && stTradeData.nMCount[i] == 0)
                            {
                                stTradeData.nState[i] = 30;
                                stTradeData.nMCount[i] = nTimeCount;
                                stTradeData.nMTime[i, stTradeData.nMCount[i]] = nHour * 100 + nMinute;
                            }
                            else if (stTradeData.nState[i] >= 30 && stTradeData.nMCount[i] < nTimeCount)
                            {
                                if (m_bNextDayChcek == true && stTradeData.nState[i] == 30)
                                {
                                    axKHOpenAPI.SetInputValue("종목코드", stTradeData.sCode[i]);
                                    axKHOpenAPI.SetInputValue("기준일자", System.DateTime.Now.ToString("yyyyMMdd"));
                                    axKHOpenAPI.SetInputValue("수정주가구분", "1");
                                    axKHOpenAPI.SetInputValue("틱범위", "5");

                                    m_bNextDayChcek = false;
                                    int nRet = axKHOpenAPI.CommRqData("주식분봉차트조회", "OPT10080", 0, GetScrNum());
                                }

                            }

                            if (stTradeData.nState[i] == 31 && nNowTime > 914)
                            {
                                int nHighPrice = 0;
                                int a = 0;

                                for (a = 0; a < 5; a++)
                                {
                                    if (stTradeData.nMHighPrice[i, a] > nHighPrice)
                                    {
                                        nHighPrice = stTradeData.nMHighPrice[i, a];
                                    }

                                    if (a == 4 && stTradeData.nClosePrice[i] + (stTradeData.nClosePrice[i] * 0.1) < nHighPrice)
                                    {
                                        stTradeData.nState[i] = 32;
                                    }
                                }

                                long lVol1 = 0;
                                long lVol2 = 0;
                                int nStart3 = 0;
                                int nEnd3 = 0;
                                int nStart2 = 0;
                                int nEnd2 = 0;
                                int nEnd1 = 0;
                                bool bCheck2 = false;

                                for (a = nTimeCount - 15; a < nTimeCount; a++)
                                {
                                    if (stTradeData.nMHighPrice[i, a] > nHighPrice)
                                    {
                                        nHighPrice = stTradeData.nMHighPrice[i, a];
                                    }

                                    if (a < nTimeCount - 10)
                                    {
                                        lVol1 += stTradeData.lMTradVol[i, a];
                                    }
                                    else if (a < nTimeCount - 5)
                                    {
                                        lVol2 += stTradeData.lMTradVol[i, a];
                                    }

                                    if (a == nTimeCount - 15)
                                    {
                                        nStart3 = stTradeData.nMStartPrice[i, a];
                                    }
                                    if (a == nTimeCount - 11)
                                    {
                                        nEnd3 = stTradeData.nMEndPrice[i, a];
                                    }
                                    if (a == nTimeCount - 10)
                                    {
                                        nStart2 = stTradeData.nMStartPrice[i, a];
                                    }
                                    if (a == nTimeCount - 6)
                                    {
                                        nEnd2 = stTradeData.nMEndPrice[i, a];
                                    }
                                    if (a == nTimeCount - 1)
                                    {
                                        nEnd1 = stTradeData.nMEndPrice[i, a];
                                    }

                                    if (a == nTimeCount - 1 && stTradeData.nClosePrice[i] + (stTradeData.nClosePrice[i] * 0.12) < nHighPrice)
                                    {
                                        stTradeData.nState[i] = 32;
                                    }
                                }

                                if (stTradeData.nState[i] == 32)
                                {
                                    break;
                                }

                                if (/*lVol1 * 2 < lVol2 &&*/ nStart2 + (nStart2 * 0.3) < nEnd2)
                                {
                                    bCheck2 = true;
                                }
                                else
                                {
                                    break;
                                }

                                if (bCheck2 == true)
                                {
                                    if ((nStart2 + nEnd2) / 2 > nEnd1)
                                    {
                                        stTradeData.nState[i] = 32;
                                        break;
                                    }

                                    if (nStart3 + (nStart3 * 0.3) > nEnd3 && nStart3 < nEnd3)
                                    {
                                        if ((nEnd2 > nEnd1 && (nStart2 + nEnd2) / 2 < nEnd1) || (nEnd2 < nEnd1 && nEnd2 + (nEnd2 * 0.15) > nEnd1))
                                        {
                                            LogManager.WriteLine("매수타이밍체크 : " + stTradeData.sCode[i] + " lVol1 : " + lVol1.ToString() + " lVol2 : " + lVol2.ToString());
                                            LogManager.WriteLine("매수타이밍체크 : Start3 : " + nStart3.ToString() + " End3 : " + nEnd3.ToString() + " Start2 : " + nStart2.ToString() + " End2 : " + nEnd2.ToString() + " End1 : " + nEnd1.ToString());
                                            stTradeData.nState[i] = 32;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void TradeDataCheck()
        {
            bool bLogin = false;
            bool bSaveLocal = false;
            bool bStartCheck = false;

            try
            {
                while (true)
                {
                    if (m_bTradeDataCheckThread == false)
                        return;

                    int nHour = Convert.ToInt32(System.DateTime.Now.ToString("HH"));
                    int nMinute = Convert.ToInt32(System.DateTime.Now.ToString("mm"));
                    int nSecond = Convert.ToInt32(System.DateTime.Now.ToString("ss"));
                    int nNowTime = nHour * 100 + nMinute;

                    if(bLogin == false && nNowTime > 835)
                    {
                        bLogin = true;
                        axKHOpenAPI.CommConnect();

                        System.Threading.Thread.Sleep(20000);
                    }
                    
                    if(bSaveLocal == false && nNowTime > 840)
                    {
                        bSaveLocal = true;
                        axKHOpenAPI.GetConditionLoad();

                        System.Threading.Thread.Sleep(5000);
                    }    

                    if (bStartCheck == false && nNowTime > 842)
                    {
                        bStartCheck = true;

                        string strScrNum = GetScrNum();
                        axKHOpenAPI.SendCondition(strScrNum, "(단타)급등종목 검색", 0, 1);

                        _strRealConScrNum = strScrNum;
                        _strRealConName = "000^(단타)급등종목 검색";
                        _nIndex = 0;
                    }
                    
                    
                    if (nHour == 8 && nMinute == 59 && nSecond > 0 && m_bStartSell == false)
                    {
                        m_bStartSell = true;

                        axKHOpenAPI.SetInputValue("계좌번호", "5198658610");
                        axKHOpenAPI.SetInputValue("비밀번호", "");
                        axKHOpenAPI.SetInputValue("상장폐지조회구분", "0");
                        axKHOpenAPI.SetInputValue("비밀번호입력매체구분", "00");

                        axKHOpenAPI.CommRqData("계좌평가현황요청", "opw00004", 0, GetScrNum());
                    }
                    else if (nHour == 15 && nMinute == 21)
                    {               
                        LogManager.WriteLine("종가 매수 시작");

                        for (int i = 0; i < 200; i++)
                        {
                            if (m_nCloseSellCount == 10)
                                break;

                            if (stTradeData.sCode[i] != "")
                            {
                                /*
                                axKHOpenAPI.SetInputValue("종목코드", stTradeData.sCode[i]);
                                axKHOpenAPI.SetInputValue("기준일자", System.DateTime.Now.ToString("yyyyMMdd"));
                                axKHOpenAPI.SetInputValue("수정주가구분", "1");
                                //axKHOpenAPI.SetInputValue("틱범위", "1");

                                int nRet = axKHOpenAPI.CommRqData("주식분봉차트조회", "OPT10080", 0, GetScrNum());
                                if(nRet == 0)
                                    LogManager.WriteLine(stTradeData.sCode[i]);

                                System.Threading.Thread.Sleep(4000);
                                */
                                /*
                                if(stTradeData.nHighPrice[i] > 0)
                                {
                                    int nPlusPrice = stTradeData.nClosePrice[i] + Convert.ToInt32(stTradeData.nClosePrice[i] * 0.05);

                                    if(stTradeData.nHighPrice[i] > nPlusPrice && stTradeData.n3HourStartPrice[i] < stTradeData.n3HourLastPrice[i] && m_nCloseSellCount < 10 && stTradeData.nNowPrice[i] != 0)
                                    {
                                        int nQty = 1;

                                        if (stTradeData.nNowPrice[i] > 10000)
                                        {
                                            nQty = 80000 / stTradeData.nNowPrice[i];
                                        }
                                        else
                                        {
                                            nQty = 50000 / stTradeData.nNowPrice[i];
                                        }

                                        int lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "03", 0, "");
                                        if (lRet == 0)
                                        {
                                            LogManager.WriteLine("주식분봉차트조회 매수 : " + stTradeData.sCode[i]);
                                            m_nCloseSellCount++;
                                        }
                                        
                                        System.Threading.Thread.Sleep(1000);
                                    }
                                }
                                */
                            }
                        }

                        return;
                        
                    }

                    if(nNowTime >= 900)
                    {
                        m_bSale = false;
                    }

                    for (int i = 0; i < 200; i++)
                    {
                        if (stTradeData.sCode[i] != "" && (stTradeData.nState[i] == 0 || stTradeData.nState[i] == 6) && m_nTradeCount < 10 && m_bSale == true)
                        {
                            int nNowPrice = Convert.ToInt32(stTradeData.nNowPrice[i]);
                            int nHigePrice10Min = 0;
                            int nLowPrice10Min = 0;

                            if (stTradeData.nMCount[i] > 10)
                            {
                                int nHighPoint = 0;

                                for (int j = 0; j < 10; j++)
                                {
                                    if (nHigePrice10Min < stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - j])
                                    {
                                        nHigePrice10Min = stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - j];
                                        nHighPoint = j;
                                    }

                                    /*
                                    if(j > 5)
                                    {
                                        if (nLowPrice10Min == 0 && stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j] > 0)
                                        {
                                            nLowPrice10Min = stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j];
                                        }
                                        else if (nLowPrice10Min > stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j] && stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j] > 0)
                                        {
                                            nLowPrice10Min = stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j];
                                        }
                                    }
                                    */
                                }

                                for (int j = 0; j < nHighPoint; j++)
                                {
                                    if(nLowPrice10Min == 0 && stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j] > 0)
                                    {
                                        nLowPrice10Min = stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j];
                                    }
                                    else if (nLowPrice10Min > stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j] && stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j] > 0)
                                    {
                                        nLowPrice10Min = stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - j];
                                    }
                                }

                                //nLowPrice10Min = stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - 9];
                                /*
                                if (nLowPrice10Min > 0)
                                {
                                    if (nHigePrice10Min - nLowPrice10Min > nLowPrice10Min * 0.05 && stTradeData.nSellTime[i] < nNowTime)
                                    {
                                        stTradeData.nSellTime[i] = nNowTime + 40;
                                        LogManager.WriteLine("시간딜레이 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + nNowTime.ToString() + "/" + stTradeData.nSellTime[i].ToString());
                                    }
                                }
                                */
                            }
                            

                            int nQty = 1;

                            if (m_nTradeCount < 0)
                            {
                                m_nTradeCount = 0;
                            }

                            if (nNowPrice > 0)
                            {
                                if (nNowPrice > 10000)
                                {
                                    nQty = 50000 / nNowPrice;
                                }
                                else
                                {
                                    nQty = 30000 / nNowPrice;
                                }
                            }

                            int lRet = 10;

                            int sellTime = stTradeData.nSellTime[i] + 20;

                            if (sellTime % 100 > 60)
                            {
                                sellTime = sellTime + 40;
                            }

                            if (stTradeData.nMCount[i] > 2 && stTradeData.nSellCount[i] < 5  && stTradeData.nBuyTime[i] == 0 && sellTime < nNowTime)
                            {
                                if (stTradeData.bSellSignal[i] == false && stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 1] - (stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 1] * 0.05) > nNowPrice && stTradeData.nMCount[i] > 0 && nNowTime > 910 && nNowTime < 1400)
                                {
                                    stTradeData.bSellSignal[i] = true;
                                    LogManager.WriteLine("매수신호1 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 1].ToString() + "/" + nNowPrice.ToString());
                                }
                                else if (stTradeData.bSellSignal[i] == false && nHigePrice10Min - (nHigePrice10Min * 0.04) > nLowPrice10Min && stTradeData.nMCount[i] > 10 && nLowPrice10Min > 0 && nNowTime > 910 && nNowTime < 1430)
                                {
                                    stTradeData.bSellSignal[i] = true;
                                    LogManager.WriteLine("매수신호2 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + nHigePrice10Min.ToString() + "/" + nLowPrice10Min.ToString());
                                }

                                if (stTradeData.bSellSignal[i] == true && stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - 1] < stTradeData.nMLowPrice[i, stTradeData.nMCount[i]] && stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 1] < stTradeData.nMHighPrice[i, stTradeData.nMCount[i]] && stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - 1] < nNowPrice)
                                {
                                    LogManager.WriteLine("매수(4%) :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i]);

                                    stTradeData.nBuyQty[i] = 0;
                                    stTradeData.nBuyPrice[i] = nNowPrice;
                                    lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "07", 0, "");
                                }
                            }
                            
                            if (lRet == 0)
                            {
                                stTradeData.nState[i] = 7;
                                stTradeData.nOrderQty[i] = nQty;
                                stTradeData.nBuyTime[i] = nNowTime;

                                if (nNowPrice > 10000)
                                {
                                    m_nTradeCount += 2;
                                }
                                else
                                {
                                    m_nTradeCount++;
                                }
                            }
                        }
                        else if (stTradeData.sCode[i] != "" && stTradeData.nState[i] == 7)
                        {
                            if(stTradeData.nOrderQty[i] == stTradeData.nBuyQty[i])
                            {
                                int nPlusPrice = Convert.ToInt32(stTradeData.nBuyPrice[i] * 0.016);
                                int nSellPrice = stTradeData.nBuyPrice[i] + nPlusPrice;

                                if (nSellPrice >= 1000 && nSellPrice < 5000)
                                    nSellPrice = nSellPrice - (nSellPrice % 5) + 5;
                                else if (nSellPrice >= 5000 && nSellPrice < 10000)
                                    nSellPrice = nSellPrice - (nSellPrice % 10) + 10;
                                else if (nSellPrice >= 10000 && nSellPrice < 50000)
                                    nSellPrice = nSellPrice - (nSellPrice % 50) + 50;
                                else if (nSellPrice >= 50000 && nSellPrice < 100000)
                                    nSellPrice = nSellPrice - (nSellPrice % 100) + 100;
                                else if (nSellPrice >= 100000 && nSellPrice < 500000)
                                    nSellPrice = nSellPrice - (nSellPrice % 500) + 500;

                                if(stTradeData.nBuyTime[i] + 10 < nNowTime)
                                {
                                    if(stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - 2] < stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - 1] && stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 2] < stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 1])
                                    {
                                        stTradeData.nBuyTime[i] += 1;
                                    }
                                    else if(stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - 1] > stTradeData.nNowPrice[i])
                                    {
                                        LogManager.WriteLine("매도(Status7) :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + stTradeData.nBuyPrice[i].ToString() + "\t" + stTradeData.nNowPrice[i].ToString());

                                        int nQty = stTradeData.nBuyQty[i];

                                        //int lRet = SendOrder(stTradeData.sCode[i], nQty, 2, "00", nSellPrice, "");
                                        int lRet = SendOrder(stTradeData.sCode[i], nQty, 2, "07", 0, "");

                                        if (lRet == 0)
                                        {
                                            stTradeData.nState[i] = 8;
                                            stTradeData.nSellTime[i] = nNowTime;
                                        }
                                    }
                                }
                            }
                            else if((Convert.ToInt32(stTradeData.nNowPrice[i]) > stTradeData.nBuyPrice[i] + stTradeData.nBuyPrice[i] * 0.02 || stTradeData.nBuyTime[i] + 10 < nNowTime) && stTradeData.nBuyQty[i] == 0)
                            {
                                LogManager.WriteLine("매수취소 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i]);

                                int lRet = SendOrder(stTradeData.sCode[i], stTradeData.nOrderQty[i], 3, "00", 0, stTradeData.sOrderNo[i]);

                                if (lRet == 0)
                                {
                                    stTradeData.nState[i] = 6;

                                    stTradeData.bSellSignal[i] = false;
                                    stTradeData.nBuyQty[i] = 0;
                                    stTradeData.nBuyPrice[i] = 0;
                                    stTradeData.nBuyTime[i] = 0;
                                    stTradeData.nSellTime[i] = nNowTime;

                                    if (Convert.ToInt32(stTradeData.nNowPrice[i]) > 10000)
                                    {
                                        m_nTradeCount -= 2;
                                    }
                                    else
                                    {
                                        m_nTradeCount--;
                                    }
                                }
                            }
                        }
                        else if(stTradeData.sCode[i] != "" && stTradeData.nState[i] == 8)
                        {
                            int nNowPrice = Convert.ToInt32(stTradeData.nNowPrice[i]);

                            if (stTradeData.nSellTime[i] + 1 < nNowTime)
                            {
                                LogManager.WriteLine("정정(Status8) :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + stTradeData.nNowPrice[i].ToString());
                                int lRet = SendOrder(stTradeData.sCode[i], stTradeData.nOrderQty[i], 3, "00", 0, stTradeData.sOrderNo[i]);
                                System.Threading.Thread.Sleep(2000);

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nOrderQty[i], 2, "07", 0, "");

                                if (lRet == 0)
                                {
                                    stTradeData.nSellTime[i] = nNowTime;
                                }
                            }
                            else if (stTradeData.nBuyPrice[i] - (stTradeData.nBuyPrice[i] * 0.03) >= nNowPrice && stTradeData.nBuyQty[i] > 0)
                            {
                                LogManager.WriteLine("손절(Status8) :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + stTradeData.nNowPrice[i].ToString());
                                int lRet = SendOrder(stTradeData.sCode[i], stTradeData.nOrderQty[i], 3, "00", 0, stTradeData.sOrderNo[i]);
                                System.Threading.Thread.Sleep(2000);

                                //lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 6, "03", stTradeData.nNowPrice[i], stTradeData.sOrderNo[i]);
                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nOrderQty[i], 2, "03", 0, "");

                                if (lRet == 0)
                                {
                                    stTradeData.nState[i] = 6;

                                    stTradeData.bSellSignal[i] = false;
                                    stTradeData.nBuyQty[i] = 0;
                                    stTradeData.nBuyPrice[i] = 0;
                                    stTradeData.nBuyTime[i] = 0;
                                    stTradeData.nSellTime[i] = nNowTime;
                                    stTradeData.nSellCount[i]++;

                                    if (stTradeData.nSellCount[i] == 2)
                                    {
                                        stTradeData.nState[i] = 9;
                                    }
                                }
                            }
                        }
                        else if (stTradeData.sCode[i] != "" && stTradeData.nState[i] == 0 && m_bSale == true)
                        {
                            int nCheckPrice = 0;

                            if (stTradeData.nHighPrice[i] > 0)
                            {
                                int nMinusPrice = Convert.ToInt32(stTradeData.nHighPrice[i] * 0.005);
                                nCheckPrice = stTradeData.nHighPrice[i] - nMinusPrice;

                                if (nCheckPrice >= 1000 && nCheckPrice < 5000)
                                    nCheckPrice = nCheckPrice - (nCheckPrice % 5);
                                else if (nCheckPrice >= 5000 && nCheckPrice < 10000)
                                    nCheckPrice = nCheckPrice - (nCheckPrice % 10);
                                else if (nCheckPrice >= 10000 && nCheckPrice < 50000)
                                    nCheckPrice = nCheckPrice - (nCheckPrice % 50);
                                else if (nCheckPrice >= 50000 && nCheckPrice < 100000)
                                    nCheckPrice = nCheckPrice - (nCheckPrice % 100);
                                else if (nCheckPrice >= 100000 && nCheckPrice < 500000)
                                    nCheckPrice = nCheckPrice - (nCheckPrice % 500);
                            }

                            int n5MinutePrice = 0;
                            int n10MinutePrice = 0;

                            for (int j = 0; j < 10; j++)
                            {
                                if (j < 5)
                                {
                                    n5MinutePrice += stTradeData.n5MinutePrice[i, j];
                                }

                                n10MinutePrice += stTradeData.n10MinutePrice[i, j];
                            }

                            int nNowPrice = Convert.ToInt32(stTradeData.nNowPrice[i]);

                            int nQty = 1;
                            
                            if (m_nTradeCount < 0)
                            {
                                m_nTradeCount = 0;
                            }
                            if(nNowPrice > 0)
                            {
                                if (nNowPrice > 10000)
                                {
                                    nQty = 80000 / nNowPrice;
                                    //m_nTradeCount += 2;
                                }
                                else
                                {
                                    nQty = 50000 / nNowPrice;
                                    //m_nTradeCount++;
                                }
                            }

                            if(m_nTradeCount > 9 || nNowPrice > (stTradeData.nClosePrice[i] + stTradeData.nClosePrice[i] * 0.05))
                            {
                                stTradeData.nState[i] = 1;
                            }
                            else if (stTradeData.nClosePrice[i] < nNowPrice && m_nTradeCount < 10 && nNowTime < 903)
                            {
                                int lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "03", 0, "");

                                if (lRet == 0)
                                {
                                    LogManager.WriteLine("매수 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t장시작 매수");

                                    stTradeData.nState[i] = 1;
                                    //stTradeData.nOrderQty[i] = nQty;
                                    //stTradeData.nBuyTime[i] = nNowTime;

                                    stTradeData.nAverageStatus[i] = 0;
                                    stTradeData.nUpAverageEnd1[i] = 0;
                                    stTradeData.nUpAverageEnd2[i] = 0;
                                    stTradeData.nUpAverageHigh1[i] = 0;
                                    stTradeData.nUpAverageHigh2[i] = 0;
                                    stTradeData.nDownAverageEnd1[i] = 0;
                                    stTradeData.nDownAverageEnd2[i] = 0;
                                    stTradeData.nDownAverageHigh1[i] = 0;
                                    stTradeData.nDownAverageHigh2[i] = 0;

                                    if (nNowPrice > 10000)
                                    {
                                        m_nTradeCount += 2;
                                    }
                                    else
                                    {
                                        m_nTradeCount++;
                                    }
                                }
                            }
                            else if (stTradeData.nClosePrice[i] < nNowPrice && stTradeData.nUpAverageEnd1[i] > 0 && stTradeData.nUpAverageEnd1[i] < stTradeData.nUpAverageEnd2[i] && m_nTradeCount < 10 && nNowTime < 1330)
                            {
                                int lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "03", 0, "");

                                if (lRet == 0)
                                {
                                    LogManager.WriteLine("매수 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tnUpAverageEnd1 - " + stTradeData.nUpAverageEnd1[i].ToString() + "\tnUpAverageEnd2 - " + stTradeData.nUpAverageEnd2[i].ToString());

                                    stTradeData.nState[i] = 1;
                                    //stTradeData.nOrderQty[i] = nQty;
                                    //stTradeData.nBuyTime[i] = nNowTime;

                                    stTradeData.nAverageStatus[i] = 0;
                                    stTradeData.nUpAverageEnd1[i] = 0;
                                    stTradeData.nUpAverageEnd2[i] = 0;
                                    stTradeData.nUpAverageHigh1[i] = 0;
                                    stTradeData.nUpAverageHigh2[i] = 0;
                                    stTradeData.nDownAverageEnd1[i] = 0;
                                    stTradeData.nDownAverageEnd2[i] = 0;
                                    stTradeData.nDownAverageHigh1[i] = 0;
                                    stTradeData.nDownAverageHigh2[i] = 0;

                                    if (nNowPrice > 10000)
                                    {
                                        m_nTradeCount += 2;
                                    }
                                    else
                                    {
                                        m_nTradeCount++;
                                    }
                                }
                            }
                            /*
                            else if (nNowTime < 930 && nNowPrice > 0 && (stTradeData.n5MinuteAverage[i] > stTradeData.n10MinuteAverage[i] || ((nHour == 9 && nMinute < 10) && stTradeData.n5MinutePrice[i,4] > stTradeData.n5MinutePrice[i, 3] && stTradeData.n5MinutePrice[i, 3] > 0)) && m_nTradeCount < 7)
                            {
                                int lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "03", 0, "");

                                if (lRet == 0)
                                {
                                    stTradeData.nState[i] = 1;
                                    stTradeData.nOrderQty[i] = nQty;
                                    stTradeData.nBuyTime[i] = nNowTime;

                                    if (nNowPrice > 10000)
                                    {
                                        m_nTradeCount += 2;
                                    }
                                    else
                                    {
                                        m_nTradeCount++;
                                    }
                                }
                            }                      
                            else if (nNowPrice > 0 && stTradeData.n5MinuteAverage[i] > stTradeData.n10MinuteAverage[i] && stTradeData.nNowPrice[i] >= nCheckPrice && stTradeData.nHighTime[i] > 0 && stTradeData.nHighTime[i] + 5 < nHour*100 + nMinute && stTradeData.nLowTime[i] + 5 < nHour * 100 + nMinute && m_nTradeCount < 9 && nNowTime < 1330)
                            {
                                int lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "03", 0, "");

                                if (lRet == 0)
                                {
                                    stTradeData.nState[i] = 1;
                                    stTradeData.nOrderQty[i] = nQty;
                                    stTradeData.nBuyTime[i] = nNowTime;

                                    if (nNowPrice > 10000)
                                    {
                                        m_nTradeCount += 2;
                                    }
                                    else
                                    {
                                        m_nTradeCount++;
                                    }
                                }

                                /*
                                if (nNowPrice > stTradeData.nClosePrice[i] + nPlusPrice)
                                {
                                    stTradeData.sCode[i] = "";
                                }
                                else if (nNowPrice > stTradeData.nClosePrice[i])
                                {
                                    int nQty = 1;
                                    int nBuyPrice = 0;

                                    if (nNowPrice >= 1000 && nNowPrice < 5000)
                                        nBuyPrice = nNowPrice - 5;
                                    else if (nNowPrice >= 5000 && nNowPrice < 10000)
                                        nBuyPrice = nNowPrice - 10;
                                    else if (nNowPrice >= 10000 && nNowPrice < 50000)
                                        nBuyPrice = nNowPrice - 50;
                                    else if (nNowPrice >= 50000 && nNowPrice < 100000)
                                        nBuyPrice = nNowPrice - 100;
                                    else if (nNowPrice >= 100000 && nNowPrice < 500000)
                                        nBuyPrice = nNowPrice - 500;

                                    if (nBuyPrice > 10000)
                                    {
                                        nQty = 20000 / nBuyPrice;
                                        //m_nTradeCount += 2;
                                    }
                                    else
                                    {
                                        nQty = 10000 / nBuyPrice;
                                        //m_nTradeCount++;
                                    }

                                    int lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "00", nBuyPrice, "");

                                    if (lRet == 0)
                                    {
                                        stTradeData.nState[i] = 1;
                                        stTradeData.nOrderQty[i] = nQty;
                                        stTradeData.nBuyTime[i] = nNowTime;
                                    }
                                }
                                
                            }
                            */
                        }
                        else if (stTradeData.sCode[i] != "" && stTradeData.nState[i] == 1)
                        {
                            //string sNowTime = System.DateTime.Now.ToString("hhmm");
                            
                            /*
                            if(stTradeData.nBuyTime[i] + 10 < nNowTime)
                            {
                                LogManager.WriteLine("매수취소 :\t" + stTradeData.sCode[i]);
                                SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 3, "00", 0, stTradeData.sOrderNo[i]);
                                stTradeData.nState[i] = 7;
                            }
                            */
                        }
                        else if (stTradeData.sCode[i] != "" && stTradeData.nState[i] == 3 && stTradeData.nNowPrice[i] != 0)
                        {
                            /*
                            int nPlusPrice = 0;
                            
                            if(nHour == 9 && nMinute <= 10)
                            {
                                nPlusPrice = Convert.ToInt32(stTradeData.nBuyPrice[i] * 0.03);
                            }
                            else if (nHour == 9 && nMinute > 10 && nMinute <= 30)
                            {
                                nPlusPrice = Convert.ToInt32(stTradeData.nBuyPrice[i] * 0.02);
                            }
                            else
                            {
                                nPlusPrice = Convert.ToInt32(stTradeData.nBuyPrice[i] * 0.015);
                            }
                            */
                            int nPlusPrice = 0;
                            nPlusPrice = Convert.ToInt32(stTradeData.nBuyPrice[i] * 0.03);

                            int lSellPrice = stTradeData.nBuyPrice[i] + nPlusPrice;

                            if (lSellPrice >= 1000 && lSellPrice < 5000)
                                lSellPrice = lSellPrice - (lSellPrice % 5);
                            else if (lSellPrice >= 5000 && lSellPrice < 10000)
                                lSellPrice = lSellPrice - (lSellPrice % 10);
                            else if (lSellPrice >= 10000 && lSellPrice < 50000)
                                lSellPrice = lSellPrice - (lSellPrice % 50);
                            else if (lSellPrice >= 50000 && lSellPrice < 100000)
                                lSellPrice = lSellPrice - (lSellPrice % 100);
                            else if (lSellPrice >= 100000 && lSellPrice < 500000)
                                lSellPrice = lSellPrice - (lSellPrice % 500);

                            if(stTradeData.nNowPrice[i] > lSellPrice)
                            {
                                stTradeData.bHighPriceCheck[i] = true;
                            }

                            int nCheckTime = 910;

                            if(stTradeData.bHighPriceCheck[i] == true)
                            {
                                nCheckTime = 900;
                            }

                            int lRet = 10;

                            //lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "00", lSellPrice, "");

                            int sellTime = stTradeData.nBuyTime[i] + 30;

                            if(sellTime % 100 > 60)
                            {
                                sellTime = sellTime + 40;
                            }

                            if(nNowTime == 900 && nSecond > 20 && stTradeData.nMStartPrice[i, stTradeData.nMCount[i]] > stTradeData.nNowPrice[i])
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tStartPrice - " + stTradeData.nMStartPrice[i, stTradeData.nMCount[i]].ToString() + "\tNowPrice - " + stTradeData.nNowPrice[i].ToString());

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "03", 0, "");
                            }
                            if (nNowTime > 900 && stTradeData.nMCount[i] > 0 && stTradeData.nMLowPrice[i, stTradeData.nMCount[i] - 1] > stTradeData.nNowPrice[i])
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tn5MinutePrice(4) - " + stTradeData.n5MinutePrice[i, 4].ToString() + "\tn5MinutePrice(3) - " + stTradeData.n5MinutePrice[i, 3].ToString());

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "07", 0, "");
                            }
                            else if(stTradeData.nHighTime[i] + 45 < nNowTime * 100 + nSecond && nNowTime > 900 && stTradeData.nHighTime[i] > 90000)
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tn5MinutePrice(4) - " + stTradeData.n5MinutePrice[i, 4].ToString() + "\tn5MinutePrice(3) - " + stTradeData.n5MinutePrice[i, 3].ToString());

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "07", 0, "");
                            }
                            else if (stTradeData.nNowPrice[i] > stTradeData.nBuyPrice[i] + stTradeData.nBuyPrice[i] * 0.03 && nNowTime*100 + nSecond >= 90003)
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tn5MinutePrice(4) - " + stTradeData.n5MinutePrice[i, 4].ToString() + "\tn5MinutePrice(3) - " + stTradeData.n5MinutePrice[i, 3].ToString());

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "07", 0, "");
                            }
                            else if (stTradeData.n5MinutePrice[i, 4] < stTradeData.n5MinutePrice[i, 3] && stTradeData.nMHighPrice[i, stTradeData.nMCount[i]-2] > stTradeData.nMHighPrice[i, stTradeData.nMCount[i]-1] && nNowTime > nCheckTime && stTradeData.nNowPrice[i] > stTradeData.nBuyPrice[i])
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tn5MinutePrice(4) - " + stTradeData.n5MinutePrice[i, 4].ToString() + "\tn5MinutePrice(3) - " + stTradeData.n5MinutePrice[i, 3].ToString());

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "07", 0, "");
                            }
                            else if (stTradeData.n5MinutePrice[i, 4] < stTradeData.n5MinutePrice[i, 3] && stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 2] > stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 1] && nNowTime > 1200 && stTradeData.nNowPrice[i] + stTradeData.nBuyPrice[i] * 0.01 > stTradeData.nBuyPrice[i])
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tn5MinutePrice(4) - " + stTradeData.n5MinutePrice[i, 4].ToString() + "\tn5MinutePrice(3) - " + stTradeData.n5MinutePrice[i, 3].ToString());

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "07", 0, "");
                            }
                            else if (stTradeData.n5MinutePrice[i, 4] < stTradeData.n5MinutePrice[i, 3] && stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 2] > stTradeData.nMHighPrice[i, stTradeData.nMCount[i] - 1] && nNowTime > 1430 && stTradeData.nNowPrice[i] + stTradeData.nBuyPrice[i] * 0.02 > stTradeData.nBuyPrice[i])
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tn5MinutePrice(4) - " + stTradeData.n5MinutePrice[i, 4].ToString() + "\tn5MinutePrice(3) - " + stTradeData.n5MinutePrice[i, 3].ToString());

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "07", 0, "");
                            }
                            else if (nNowTime >= 1518)
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tn5MinutePrice(4) - " + stTradeData.n5MinutePrice[i, 4].ToString() + "\tn5MinutePrice(3) - " + stTradeData.n5MinutePrice[i, 3].ToString());

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "03", 0, "");
                            }
                            else if(stTradeData.nNowPrice[i] < stTradeData.nBuyPrice[i] && sellTime < nNowTime && stTradeData.nBuyPrice[i] * stTradeData.nBuyQty[i] < 100000)
                            {
                                /*
                                if(stTradeData.nNowPrice[i] < stTradeData.nBuyPrice[i] - stTradeData.nBuyPrice[i] * 0.03)
                                {
                                    LogManager.WriteLine("매수 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t추가 매수");
                                    lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i] / 2, 1, "03", 0, "");
                                    //stTradeData.nBuyTime[i] = nNowTime;

                                    stTradeData.nAverageStatus[i] = 0;
                                    stTradeData.nUpAverageEnd1[i] = 0;
                                    stTradeData.nUpAverageEnd2[i] = 0;
                                    stTradeData.nUpAverageHigh1[i] = 0;
                                    stTradeData.nUpAverageHigh2[i] = 0;
                                    stTradeData.nDownAverageEnd1[i] = 0;
                                    stTradeData.nDownAverageEnd2[i] = 0;
                                    stTradeData.nDownAverageHigh1[i] = 0;
                                    stTradeData.nDownAverageHigh2[i] = 0;
                                }
                                else
                                {
                                    stTradeData.nBuyTime[i] = nNowTime;
                                }
                                */
                            }
                            /*
                            else if (stTradeData.nDownAverageEnd1[i] > 0 && stTradeData.nDownAverageEnd1[i] > stTradeData.nDownAverageEnd2[i])
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\tnDownAverageEnd1 - " + stTradeData.nDownAverageEnd1[i].ToString() + "\tnDownAverageEnd2 - " + stTradeData.nDownAverageEnd2[i].ToString());

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "03", 0, "");
                            }
                            else if(sellTime < nNowTime && stTradeData.nAverageStatus[i] == 2)
                            {
                                LogManager.WriteLine("매도 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t시간초과");
                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "03", 0, "");
                            }
                            */

                            /*
                            if (stTradeData.nStandardPrice[i] > 0 && stTradeData.nStandardTime[i] + 60 < (nHour * 10000) + (nMinute * 100) + nSecond)
                            {
                                //lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "03", Convert.ToInt32(stTradeData.nNowPrice[i]), "");
                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "00", Convert.ToInt32(stTradeData.nNowPrice[i]), "");
                            }

                            //int lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "00", lSellPrice, "");

                            if(stTradeData.nStandardPrice[i] == 0)
                            {
                                stTradeData.nState[i] = 4;
                                stTradeData.nStandardPrice[i] = stTradeData.nNowPrice[i];
                                stTradeData.nStandardTime[i] = (nHour * 10000) + (nMinute * 100) + nSecond;
                            }
                            */

                            if (lRet == 0)
                            {
                                stTradeData.nState[i] = 4;
                                stTradeData.nStandardPrice[i] = stTradeData.nNowPrice[i];
                                stTradeData.nStandardTime[i] = (nHour * 10000) + (nMinute * 100) + nSecond;
                            }
                        }
                        else if (stTradeData.sCode[i] != "" && (stTradeData.nState[i] == 4 || stTradeData.nState[i] == 5))
                        {
                            if (stTradeData.nNowPrice[i] != 0)
                            {
                                int lRet = 10;

                                /*
                                if (stTradeData.nStandardPrice[i] > 0 && stTradeData.nStandardTime[i] + 60 < (nHour * 10000) + (nMinute * 100) + nSecond)
                                {
                                    if(stTradeData.sOrderNo[i] == "")
                                    {
                                        //lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "06", 0, "");
                                        lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 2, "00", Convert.ToInt32(stTradeData.nNowPrice[i]), "");
                                    }
                                    else
                                    {
                                        lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 6, "03", stTradeData.nNowPrice[i], stTradeData.sOrderNo[i]);
                                    }                                
                                }

                                if (lRet == 0)
                                {
                                    stTradeData.nState[i] = 5;
                                    stTradeData.nStandardPrice[i] = stTradeData.nNowPrice[i];
                                    stTradeData.nStandardTime[i] = (nHour * 10000) + (nMinute * 100) + nSecond;
                                }
                                */

                               
                                int nNowPrice = Convert.ToInt32(stTradeData.nNowPrice[i]);
                                int nMinusPrice = Convert.ToInt32(stTradeData.nBuyPrice[i] * 0.02);

                                if (nHour == 9 && nMinute < 11)
                                {
                                    nMinusPrice = Convert.ToInt32(stTradeData.nBuyPrice[i] * 0.03);
                                }

                                if (nNowPrice < stTradeData.nBuyPrice[i] - nMinusPrice)
                                {
                                    LogManager.WriteLine("손절 :\t" + stTradeData.sCode[i]);
                                    lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 6, "03", nNowPrice, stTradeData.sOrderNo[i]);

                                    if (lRet == 0)
                                    {
                                        stTradeData.nState[i] = 6;
                                    }
                                }
                                /*
                                else if (stTradeData.bEndSell[i] == true && nNowTime > 930)
                                {
                                    int lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 6, "00", stTradeData.nHighPrice[i], stTradeData.sOrderNo[i]);
                                    if (lRet == 0)
                                    {
                                        LogManager.WriteLine("가격조정 :\t" + stTradeData.sCode[i]);
                                        stTradeData.bEndSell[i] = false;
                                    }
                                }
                                */

                                if (nHour == 15 && nMinute > 14)
                                {
                                    LogManager.WriteLine("장마감손절 :\t" + stTradeData.sCode[i]);
                                    lRet = SendOrder(stTradeData.sCode[i], stTradeData.nBuyQty[i], 6, "03", stTradeData.nNowPrice[i], stTradeData.sOrderNo[i]);

                                    if (lRet == 0)
                                    {
                                        stTradeData.nState[i] = 6;
                                    }
                                }
                                else if (nHour == 15 && nMinute < 6 && stTradeData.n3HourStartPrice[i] < stTradeData.nNowPrice[i])
                                {
                                    stTradeData.n3HourStartPrice[i] = stTradeData.nNowPrice[i];
                                }
                                else if (nHour == 15 && nMinute == 19 && stTradeData.n3HourLastPrice[i] < stTradeData.nNowPrice[i])
                                {
                                    stTradeData.n3HourLastPrice[i] = stTradeData.nNowPrice[i];
                                }
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private int SendOrder(string sOrderCode, int nQty, int nOrderType, string sOrderKind, int nTradePrice, string sOrgOrderNo)
        {
            // =================================================
            // 계좌번호 입력 여부 확인
            /*
            if (cbo계좌.Text.Length != 10)
            {
                Logger(Log.에러, "계좌번호 10자리를 입력해 주세요");

                return;
            }
            */

            //cbo계좌.Text = "5198658610";

            // =================================================
            // 종목코드 입력 여부 확인
            /*
            if (txt주문종목코드.TextLength != 6)
            {
                Logger(Log.에러, "종목코드 6자리를 입력해 주세요");

                return;
            }
            */

            //txt주문종목코드.Text = sOrderCode;

            // =================================================
            // 주문수량 입력 여부 확인
            int n주문수량 = nQty;

            /*
            if (txt주문수량.TextLength > 0)
            {
                n주문수량 = Int32.Parse(txt주문수량.Text.Trim());
            }
            else
            {
                Logger(Log.에러, "주문수량을 입력하지 않았습니다");

                return;
            }

            if (n주문수량 < 1)
            {
                Logger(Log.에러, "주문수량이 1보다 작습니다");

                return;
            }
            */

            // =================================================
            // 거래구분 취득
            // 0:지정가, 3:시장가, 5:조건부지정가, 6:최유리지정가, 7:최우선지정가,
            // 10:지정가IOC, 13:시장가IOC, 16:최유리IOC, 20:지정가FOK, 23:시장가FOK,
            // 26:최유리FOK, 61:장개시전시간외, 62:시간외단일가매매, 81:시간외종가

            string s거래구분 = sOrderKind;
            //s거래구분 = KOACode.hogaGb[cbo거래구분.SelectedIndex].code;

            // =================================================
            // 주문가격 입력 여부

            int n주문가격 = nTradePrice;

            /*
            if (txt주문가격.TextLength > 0)
            {
                n주문가격 = Int32.Parse(txt주문가격.Text.Trim());
            }

            if (s거래구분 == "3" || s거래구분 == "13" || s거래구분 == "23" && n주문가격 < 1)
            {
                Logger(Log.에러, "주문가격이 1보다 작습니다");
            }
            */

            // =================================================
            // 매매구분 취득
            // (1:신규매수, 2:신규매도 3:매수취소, 
            // 4:매도취소, 5:매수정정, 6:매도정정)

            int n매매구분 = nOrderType;
            //n매매구분 = KOACode.orderType[cbo매매구분.SelectedIndex].code;

            // =================================================
            // 원주문번호 입력 여부

            //txt원주문번호.Text = sOrgOrderNo;

            /*
            if (n매매구분 > 2 && txt원주문번호.TextLength < 1)
            {
                Logger(Log.에러, "원주문번호를 입력해주세요");
            }
            */

            // =================================================
            // 주식주문
            int lRet;

            lRet = axKHOpenAPI.SendOrder("주식주문", GetScrNum(), "5198658610",
                                        n매매구분, sOrderCode, n주문수량,
                                        n주문가격, s거래구분, sOrgOrderNo);

            return lRet;

            if (lRet == 0)
            {
                //Logger(Log.일반, "주문이 전송 되었습니다");
            }
            else
            {
                //Logger(Log.에러, "주문이 전송 실패 하였습니다. [에러] : " + lRet);
            }
        }

        // 로그를 출력합니다.
        public void Logger(Log type, string format, params Object[] args)
        {
            string message = String.Format(format, args);

            switch (type)
            {
                case Log.조회:
                    lst조회.Items.Add(message);
                    lst조회.SelectedIndex = lst조회.Items.Count - 1;
                    break;
                case Log.에러:
                    lst에러.Items.Add(message);
                    lst에러.SelectedIndex = lst에러.Items.Count - 1;
                    break;
                case Log.일반:
                    lst일반.Items.Add(message);
                    lst일반.SelectedIndex = lst일반.Items.Count - 1;
                    break;
                case Log.실시간:
                    lst실시간.Items.Add(message);
                    lst실시간.SelectedIndex = lst실시간.Items.Count - 1;
                    break;
                default:
                    break;
            }
        }

        // 로그인 창을 엽니다.
        private void 로그인ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (axKHOpenAPI.CommConnect() == 0)
            {
                Logger(Log.일반, "로그인창 열기 성공");
            }
            else
            {
                Logger(Log.에러, "로그인창 열기 실패");
            }
        }

        // 샘플 프로그램을 종료 합니다.
        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_bLiveCheckThread = false;
            m_bTradeDataCheckThread = false;

            DisconnectAllRealData();
            axKHOpenAPI.CommTerminate();
            Logger(Log.일반, "로그아웃");
            System.Threading.Thread.Sleep(2000);

            Application.Exit();
        }

        // 로그아웃
        private void 로그아웃ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisconnectAllRealData();
            axKHOpenAPI.CommTerminate();
            Logger(Log.일반, "로그아웃");
        }

        // 접속상태확인
        private void 접속상태ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (axKHOpenAPI.GetConnectState() == 0)
            {
                Logger(Log.일반, "Open API 연결 : 미연결");
            }
            else
            {
                Logger(Log.일반, "Open API 연결 : 연결중");
            }
        }

        private void axKHOpenAPI_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (e.sRQName == "주식주문")
            {
                /*
                string s원주문번호 = axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "").Trim();

                long n원주문번호 = 0;
                bool canConvert = long.TryParse(s원주문번호, out n원주문번호);

                if (canConvert == true)
                    txt원주문번호.Text = s원주문번호;
                //else
                //    Logger(Log.에러, "잘못된 원주문번호 입니다");
                */

            }
            // OPT1001 : 주식기본정보
            else if (e.sRQName == "주식기본정보")
            {
                //m_sNowPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "현재가").Trim();

                /*
                Logger(Log.조회, "{0} | 현재가:{1:N0} | 등락율:{2} | 거래량:{3:N0} ",
                       axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, 0, "종목명").Trim(),
                       Int32.Parse(axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "현재가").Trim()),
                       axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "등락율").Trim(),
                       Int32.Parse(axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "거래량").Trim()));
                */
            }
            // OPT10081 : 주식일봉차트조회
            else if (e.sRQName == "주식일봉차트조회")
            {
                //LogManager.WriteLine("주식일봉차트조회 시작");
                int nCnt = axKHOpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);
                if(nCnt > 0)
                {
                    string sCode = axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "종목코드");
                    sCode = sCode.Replace(" ", "");
                    string sLastPrice = axKHOpenAPI.GetMasterLastPrice(sCode);
                    int nLsatPrice = Convert.ToInt32(sLastPrice);

                    int nCodeCount = 0;
                    for (int j = 0; j < 200; j++)
                    {
                        if (sCode == stTradeData.sCode[j])
                        {
                            nCodeCount = j;
                            break;
                        }
                    }

                    //LogManager.WriteLine("주식일봉차트조회 : " + sCode);

                    string sTime = axKHOpenAPI.GetCommData(e.sTrCode, "", 1, "일자");

                    string sEndPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", 1, "현재가");
                    sEndPrice = sEndPrice.Replace(" ", "");
                    sEndPrice = sEndPrice.Replace("+", "");
                    sEndPrice = sEndPrice.Replace("-", "");

                    string sHighPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", 1, "고가");
                    sHighPrice = sHighPrice.Replace(" ", "");
                    sHighPrice = sHighPrice.Replace("+", "");
                    sHighPrice = sHighPrice.Replace("-", "");

                    string sLowPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", 1, "저가");
                    sLowPrice = sLowPrice.Replace(" ", "");
                    sLowPrice = sLowPrice.Replace("+", "");
                    sLowPrice = sLowPrice.Replace("-", "");

                    stTradeData.nPivot[nCodeCount] = ((Convert.ToInt32(sHighPrice) + Convert.ToInt32(sLowPrice) + Convert.ToInt32(sEndPrice)) / 3) + Convert.ToInt32(sHighPrice) - Convert.ToInt32(sLowPrice);

                    if (Convert.ToInt32(sEndPrice) >= 1000 && Convert.ToInt32(sEndPrice) < 5000)
                    {
                        stTradeData.nPivot[nCodeCount] = stTradeData.nPivot[nCodeCount] - (stTradeData.nPivot[nCodeCount] % 10);
                    }
                    else if (Convert.ToInt32(sEndPrice) >= 5000 && Convert.ToInt32(sEndPrice) < 10000)
                    {
                        stTradeData.nPivot[nCodeCount] = stTradeData.nPivot[nCodeCount] - (stTradeData.nPivot[nCodeCount] % 20);
                    }
                    else if (Convert.ToInt32(sEndPrice) >= 10000 && Convert.ToInt32(sEndPrice) < 50000)
                    {
                        stTradeData.nPivot[nCodeCount] = stTradeData.nPivot[nCodeCount] - (stTradeData.nPivot[nCodeCount] % 100);
                    }
                    else if (Convert.ToInt32(sEndPrice) >= 50000 && Convert.ToInt32(sEndPrice) < 100000)
                    {
                        stTradeData.nPivot[nCodeCount] = stTradeData.nPivot[nCodeCount] - (stTradeData.nPivot[nCodeCount] % 200);
                    }
                    else if (Convert.ToInt32(sEndPrice) >= 100000 && Convert.ToInt32(sEndPrice) < 500000)
                    {
                        stTradeData.nPivot[nCodeCount] = stTradeData.nPivot[nCodeCount] - (stTradeData.nPivot[nCodeCount] % 1000);
                    }

                    string sTodayHighPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "고가");
                    sTodayHighPrice = sTodayHighPrice.Replace(" ", "");
                    sTodayHighPrice = sTodayHighPrice.Replace("+", "");
                    sTodayHighPrice = sTodayHighPrice.Replace("-", "");

                    if (Convert.ToInt32(sTodayHighPrice) > Convert.ToInt32(sEndPrice) + Convert.ToInt32(sEndPrice) * 0.1)
                    {
                        stTradeData.nState[nCodeCount] = 32;
                        axKHOpenAPI.SetRealRemove("ALL", sCode);  // 모든 화면에서 실시간 해지
                        LogManager.WriteLine("10%이상 상승 : " + stTradeData.sCode[nCodeCount]);
                    }
                }

                System.Threading.Thread.Sleep(4000);
                m_bNextDayChcek = true;

                /*
                int nCnt = axKHOpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);
                if(nCnt > 5)
                {
                    long trVol1, trVol2, trVol3, trVol4, trVol5;

                    trVol1 = Convert.ToInt64(axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "거래량"));
                    trVol2 = Convert.ToInt64(axKHOpenAPI.GetCommData(e.sTrCode, "", 1, "거래량"));
                    trVol3 = Convert.ToInt64(axKHOpenAPI.GetCommData(e.sTrCode, "", 2, "거래량"));
                    trVol4 = Convert.ToInt64(axKHOpenAPI.GetCommData(e.sTrCode, "", 3, "거래량"));
                    trVol5 = Convert.ToInt64(axKHOpenAPI.GetCommData(e.sTrCode, "", 4, "거래량"));

                    bool bSave = false;

                    if (trVol1 > trVol2 * 2 && trVol1 > trVol3 * 2 && trVol1 > trVol4 * 2 && trVol1 > trVol5 * 2)
                    {
                        bSave = true;
                    }
                    else if (trVol2 > trVol1 * 2 && trVol2 > trVol3 * 2 && trVol2 > trVol4 * 2 && trVol2 > trVol5 * 2)
                    {
                        bSave = true;
                    }

                    if (bSave == true)
                    {
                        StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\DayCheck.txt", true);
                        string sCode = axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "종목코드");
                        sw.WriteLine(sCode.Trim());
                        sw.Flush();
                        sw.Close();
                    } 
                }
                */


                /*
                for (int i = 0; i < nCnt; i++)
                {
                    
                    Logger(Log.조회, "{0} | 현재가:{1:N0} | 거래량:{2:N0} | 시가:{3:N0} | 고가:{4:N0} | 저가:{5:N0} ",
                        axKHOpenAPI.GetCommData(e.sTrCode, "", i, "일자").Trim(),
                        Int32.Parse(axKHOpenAPI.GetCommData(e.sTrCode, "", i, "현재가").Trim()),
                        Int32.Parse(axKHOpenAPI.GetCommData(e.sTrCode, "", i, "거래량").Trim()),
                        Int32.Parse(axKHOpenAPI.GetCommData(e.sTrCode, "", i, "시가").Trim()),
                        Int32.Parse(axKHOpenAPI.GetCommData(e.sTrCode, "", i, "고가").Trim()),
                        Int32.Parse(axKHOpenAPI.GetCommData(e.sTrCode, "", i, "저가").Trim()));  
                }
                */
            }
            else if (e.sRQName == "계좌평가현황요청")
            {
                int nCnt = axKHOpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);
                //object candleObject = axKHOpenAPI.GetCommDataEx(e.sTrCode, "주식분봉차트조회");

                //string aa = axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "예수금");
                //m_nAllAsset = Convert.ToInt32(axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "예수금")) - Convert.ToInt32(axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "총매입금액"));

                for (int i = 0; i < nCnt; i++)
                {
                    string sCode = axKHOpenAPI.GetCommData(e.sTrCode, "", i, "종목코드");

                    if(sCode != "")
                    {
                        sCode = sCode.Substring(1, sCode.Length - 1);
                        sCode = sCode.TrimEnd(' ');

                        int nQty = Convert.ToInt32(axKHOpenAPI.GetCommData(e.sTrCode, "", i, "보유수량"));
                        int nPrice = Convert.ToInt32(axKHOpenAPI.GetCommData(e.sTrCode, "", i, "평균단가"));

                        AddTradeList(sCode + ";", 4, nQty, nPrice);
                        System.Threading.Thread.Sleep(2000);
                    }
                    
                    //m_nStartSellCount++;

                    /*
                    int lSellPrice = Convert.ToInt32(axKHOpenAPI.GetCommData(e.sTrCode, "", i, "평균단가"));
                    int nPlusPrice = Convert.ToInt32(lSellPrice * 0.045);

                    lSellPrice = lSellPrice + nPlusPrice;

                    if (lSellPrice >= 1000 && lSellPrice < 5000)
                        lSellPrice = lSellPrice - (lSellPrice % 5);
                    else if (lSellPrice >= 5000 && lSellPrice < 10000)
                        lSellPrice = lSellPrice - (lSellPrice % 10);
                    else if (lSellPrice >= 10000 && lSellPrice < 50000)
                        lSellPrice = lSellPrice - (lSellPrice % 50);
                    else if (lSellPrice >= 50000 && lSellPrice < 100000)
                        lSellPrice = lSellPrice - (lSellPrice % 100);
                    else if (lSellPrice >= 100000 && lSellPrice < 500000)
                        lSellPrice = lSellPrice - (lSellPrice % 500);

                    AddTradeList(sCode + ";", 4, nQty);
                    System.Threading.Thread.Sleep(1000);
                    m_nStartSellCount++;

                    int waitCount = 0;
                    while(true)
                    {
                        if (stTradeData.nNowPrice[i] != 0 || waitCount == 50)
                            break;

                        waitCount++;
                        System.Threading.Thread.Sleep(100);
                    }

                    if(stTradeData.nNowPrice[i] != 0 && stTradeData.nNowPrice[i] < stTradeData.nClosePrice[i])
                    {
                        lSellPrice = stTradeData.nNowPrice[i];
                    }

                    int lRet = SendOrder(sCode, nQty, 2, "00", lSellPrice, "");
                    */
                }

            }
            else if (e.sRQName == "주식분봉차트조회")
            {
                //LogManager.WriteLine("주식분봉차트조회 시작");
                int nCnt = axKHOpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);
                if(nCnt > 0)
                {
                    string sCode = axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "종목코드");
                    sCode = sCode.Replace(" ", "");
                    string sLastPrice = axKHOpenAPI.GetMasterLastPrice(sCode);
                    int nLsatPrice = Convert.ToInt32(sLastPrice);

                    int nCodeCount = 0;
                    for (int j = 0; j < 200; j++)
                    {
                        if (sCode == stTradeData.sCode[j])
                        {
                            nCodeCount = j;
                            break;
                        }
                    }

                    //LogManager.WriteLine("주식분봉차트조회 : " + sCode);

                    string sCheckTime = System.DateTime.Now.ToString("yyyyMMdd") + "091000";
                    int nHighPrice = 0;
                    int nNowPrice = 0;
                    int nBuyPrice = 0;

                    for (int i = 0; i < nCnt; i++)
                    {
                        string sTime = axKHOpenAPI.GetCommData(e.sTrCode, "", i, "체결시간");
                        string sHour = sTime.Substring(14, 2);
                        string sMinute = sTime.Substring(16, 2);
                        int nTimeCount = (Convert.ToInt32(sHour) - 9) * 60 + Convert.ToInt32(sMinute);

                        if (nTimeCount > 0)
                        {
                            nTimeCount = nTimeCount / 5;
                        }

                        string sStartPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", i, "시가");
                        sStartPrice = sStartPrice.Replace(" ", "");
                        sStartPrice = sStartPrice.Replace("+", "");
                        sStartPrice = sStartPrice.Replace("-", "");

                        string sEndPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", i, "현재가");
                        sEndPrice = sEndPrice.Replace(" ", "");
                        sEndPrice = sEndPrice.Replace("+", "");
                        sEndPrice = sEndPrice.Replace("-", "");

                        string sHighPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", i, "고가");
                        sHighPrice = sHighPrice.Replace(" ", "");
                        sHighPrice = sHighPrice.Replace("+", "");
                        sHighPrice = sHighPrice.Replace("-", "");

                        string sLowPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", i, "저가");
                        sLowPrice = sLowPrice.Replace(" ", "");
                        sLowPrice = sLowPrice.Replace("+", "");
                        sLowPrice = sLowPrice.Replace("-", "");

                        string sTradeVol = axKHOpenAPI.GetCommData(e.sTrCode, "", i, "거래량");
                        sTradeVol = sTradeVol.Replace(" ", "");
                        sTradeVol = sTradeVol.Replace("+", "");
                        sTradeVol = sTradeVol.Replace("-", "");


                        stTradeData.nMStartPrice[nCodeCount, nTimeCount] = Convert.ToInt32(sStartPrice);
                        stTradeData.nMEndPrice[nCodeCount, nTimeCount] = Convert.ToInt32(sEndPrice);
                        stTradeData.nMHighPrice[nCodeCount, nTimeCount] = Convert.ToInt32(sHighPrice);
                        stTradeData.nMLowPrice[nCodeCount, nTimeCount] = Convert.ToInt32(sLowPrice);
                        stTradeData.nMTime[nCodeCount, nTimeCount] = Convert.ToInt32(sHour) * 100 + Convert.ToInt32(sMinute);
                        //stTradeData.nMHighTime[i, nTimeCount] = 0;
                        //stTradeData.nMLowTime[i, nTimeCount] = 0;
                        stTradeData.lMTradVol[nCodeCount, nTimeCount] = Convert.ToInt32(sTradeVol);
                        //stTradeData.lMTradVolAll[i, nTimeCount] = 0;

                        if (stTradeData.nMTime[nCodeCount, nTimeCount] == 900)
                        {
                            break;
                        }

                        /*
                        if(i == 0)
                        {
                            string sBuyPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", i, "현재가");
                            sBuyPrice = sBuyPrice.Replace(" ", "");
                            sBuyPrice = sBuyPrice.Replace("+", "");
                            sBuyPrice = sBuyPrice.Replace("-", "");

                            nBuyPrice = Convert.ToInt32(sBuyPrice);
                        }
                        //else if (sTime.Contains(sCheckTime))
                        else if(Convert.ToInt64(sTime) <= Convert.ToInt64(sCheckTime) && Convert.ToInt64(sTime) > Convert.ToInt64(sCheckTime) - 1000)
                        {
                            string sHighPrice = axKHOpenAPI.GetCommData(e.sTrCode, "", i, "고가");
                            sHighPrice = sHighPrice.Replace(" ", "");
                            sHighPrice = sHighPrice.Replace("+", "");
                            sHighPrice = sHighPrice.Replace("-", "");

                            nNowPrice = Convert.ToInt32(sHighPrice);

                            if(nHighPrice < nNowPrice)
                            {
                                nHighPrice = nNowPrice;
                            }
                        }
                        */
                    }

                    stTradeData.nState[nCodeCount] = 31;
                }

                System.Threading.Thread.Sleep(5000);
                m_bNextMinChcek = true;

                /*
                for (int j = 0; j < 200; j++)
                {
                    if(sCode == stTradeData.sCode[j])
                    {
                        if (stTradeData.nClosePrice[j] + stTradeData.nClosePrice[j] * 0.05 < nHighPrice && m_nCloseSellCount < 10)
                        {
                            int nQty = 1;

                            if (nNowPrice > 10000)
                            {
                                nQty = 80000 / nNowPrice;
                            }
                            else
                            {
                                nQty = 50000 / nNowPrice;
                            }

                            int lRet = SendOrder(sCode, nQty, 1, "03", 0, "");
                            if (lRet == 0)
                                LogManager.WriteLine("주식분봉차트조회 매수 : " + sCode);
                            m_nCloseSellCount++;

                            break;
                        }
                    }
                }
                */

                /*
                if (nHighPrice - nLsatPrice > nLsatPrice * 0.05 && m_nCloseSellCount < 10)
                {
                    int nQty = 1;

                    if (nNowPrice > 10000)
                    {
                        nQty = 20000 / nNowPrice;
                    }
                    else
                    {
                        nQty = 10000 / nNowPrice;
                    }

                    int lRet = SendOrder(sCode, nQty, 1, "03", 0, "");
                    if(lRet == 0)
                        LogManager.WriteLine("주식분봉차트조회 매수 : " + sCode);
                    m_nCloseSellCount++;
                }
                */
            }
        }

        private void axKHOpenAPI_OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (Error.IsError(e.nErrCode))
            {
                Logger(Log.일반, "[로그인 처리결과] " + Error.GetErrorMessage());
            }
            else
            {
                Logger(Log.에러, "[로그인 처리결과] " + Error.GetErrorMessage());
            }
        }

        private void axKHOpenAPI_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if (e.sGubun == "0")
            {
                Logger(Log.실시간, "구분 : 주문체결통보");
                Logger(Log.실시간, "주문/체결시간 : " + axKHOpenAPI.GetChejanData(908));
                Logger(Log.실시간, "종목명 : " + axKHOpenAPI.GetChejanData(302));
                Logger(Log.실시간, "주문수량 : " + axKHOpenAPI.GetChejanData(900));
                Logger(Log.실시간, "주문가격 : " + axKHOpenAPI.GetChejanData(901));
                Logger(Log.실시간, "체결수량 : " + axKHOpenAPI.GetChejanData(911));
                Logger(Log.실시간, "체결가격 : " + axKHOpenAPI.GetChejanData(910));
                Logger(Log.실시간, "=======================================");

                if(axKHOpenAPI.GetChejanData(907).Trim() == "2" || axKHOpenAPI.GetChejanData(907).Trim() == "매수")
                {
                    int nState = 0;
                    int nBuyQty = 0;
                    int nBuyPrice = 0;

                    if (axKHOpenAPI.GetChejanData(913).Trim() == "접수")
                    {
                        nState = 1;
                        nBuyQty = Convert.ToInt32(axKHOpenAPI.GetChejanData(900).Trim());
                        nBuyPrice = Convert.ToInt32(axKHOpenAPI.GetChejanData(901).Trim());
                    }
                    else if (axKHOpenAPI.GetChejanData(913).Trim() == "확인")
                    {
                        nState = 2;
                        nBuyQty = Convert.ToInt32(axKHOpenAPI.GetChejanData(900).Trim());
                        nBuyPrice = Convert.ToInt32(axKHOpenAPI.GetChejanData(901).Trim());
                    }
                    else if (axKHOpenAPI.GetChejanData(913).Trim() == "체결")
                    {
                        nState = 3;
                        nBuyQty = Convert.ToInt32(axKHOpenAPI.GetChejanData(911).Trim());
                        nBuyPrice = Convert.ToInt32(axKHOpenAPI.GetChejanData(910).Trim());

                        /*
                        if (nBuyPrice > 20000)
                            m_nTradeCount += 2;
                        else
                            m_nTradeCount++;
                        */
                    }

                    ChangeTradeList(axKHOpenAPI.GetChejanData(302).Trim(), nState, axKHOpenAPI.GetChejanData(9203).Trim(), nBuyQty, nBuyPrice, 0, 0);
                }
                else if (axKHOpenAPI.GetChejanData(907).Trim() == "1" || axKHOpenAPI.GetChejanData(907).Trim() == "매도")
                {
                    int nState = 0;
                    int nSellQty = 0;
                    int nSellPrice = 0;

                    if (axKHOpenAPI.GetChejanData(913).Trim() == "접수")
                    {
                        nState = 4;
                        nSellQty = Convert.ToInt32(axKHOpenAPI.GetChejanData(900).Trim());
                        nSellPrice = Convert.ToInt32(axKHOpenAPI.GetChejanData(901).Trim());
                    }
                    else if (axKHOpenAPI.GetChejanData(913).Trim() == "확인")
                    {
                        nState = 5;
                        nSellQty = Convert.ToInt32(axKHOpenAPI.GetChejanData(900).Trim());
                        nSellPrice = Convert.ToInt32(axKHOpenAPI.GetChejanData(901).Trim());
                    }
                    else if (axKHOpenAPI.GetChejanData(913).Trim() == "체결")
                    {
                        nState = 6;
                        nSellQty = Convert.ToInt32(axKHOpenAPI.GetChejanData(911).Trim());
                        nSellPrice = Convert.ToInt32(axKHOpenAPI.GetChejanData(910).Trim());

                        //m_nAllAsset = m_nAllAsset + (nSellQty * nSellPrice);
                        

                        if (nSellPrice > 20000)
                            m_nTradeCount -= 2;
                        else
                            m_nTradeCount--;

                        if (m_nTradeCount < 0)
                        {
                            m_nTradeCount = 0;
                        }
                        
                    }

                    ChangeTradeList(axKHOpenAPI.GetChejanData(302).Trim(), nState, axKHOpenAPI.GetChejanData(9203).Trim(), 0, 0, nSellQty, nSellPrice);
                }
            }
            else if (e.sGubun == "1")
            {
                Logger(Log.실시간, "구분 : 잔고통보");
            }
            else if (e.sGubun == "3")
            {
                Logger(Log.실시간, "구분 : 특이신호");
            }

        }

        private void axKHOpenAPI_OnReceiveMsg(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            Logger(Log.조회, "===================================================");
            Logger(Log.조회, "화면번호:{0} | RQName:{1} | TRCode:{2} | 메세지:{3}", e.sScrNo, e.sRQName, e.sTrCode, e.sMsg);
            LogManager.WriteLine("RQName: " + e.sRQName + " TRCode: " + e.sTrCode + " 메세지: " + e.sMsg);
        }

        private void axKHOpenAPI_OnReceiveRealData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            for (int i = 0; i < 200; i++)
            {
                if (stTradeData.sCode[i] == e.sRealKey)
                {
                    string sNowPrice = axKHOpenAPI.GetCommRealData(e.sRealType, 10).Trim(); //현재가
                    sNowPrice = sNowPrice.Replace("+","");
                    sNowPrice = sNowPrice.Replace("-", "");

                    string sNowTradeVol = axKHOpenAPI.GetCommRealData(e.sRealType, 15).Trim();  //거래량
                    sNowTradeVol = sNowTradeVol.Replace("+", "");
                    sNowTradeVol = sNowTradeVol.Replace("-", "");

                    string sAddTradeVol = axKHOpenAPI.GetCommRealData(e.sRealType, 13).Trim();  //누적거래량
                    sAddTradeVol = sAddTradeVol.Replace("+", "");
                    sAddTradeVol = sAddTradeVol.Replace("-", "");

                    if (sNowPrice != "")
                    {
                        stTradeData.nNowPrice[i] = Convert.ToInt32(sNowPrice);

                        int nHour = Convert.ToInt32(System.DateTime.Now.ToString("HH"));
                        int nMinute = Convert.ToInt32(System.DateTime.Now.ToString("mm"));
                        int nSecond = Convert.ToInt32(System.DateTime.Now.ToString("ss"));
                        int nNowTime = nHour * 100 + nMinute;

                        int nTimeCount = (nHour - 9) * 60 + nMinute;

                        if (nTimeCount > 0)
                        {
                            nTimeCount = nTimeCount / 5;
                        }

                        if (stTradeData.nMTime[i, nTimeCount] == 0)
                        {
                            stTradeData.nMTime[i, nTimeCount] = nNowTime - (nNowTime % 5);
                            stTradeData.nMStartPrice[i, nTimeCount] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMEndPrice[i, nTimeCount] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMHighPrice[i, nTimeCount] = Convert.ToInt32(sNowPrice);
                            //stTradeData.nMHighTime[i, nTimeCount] = nSecond;
                            stTradeData.nMLowPrice[i, nTimeCount] = Convert.ToInt32(sNowPrice);
                            //stTradeData.nMLowTime[i, nTimeCount] = nSecond;
                            stTradeData.lMTradVol[i, nTimeCount] = Convert.ToInt64(sNowTradeVol);
                            stTradeData.lMTradVolAll[i, nTimeCount] = Convert.ToInt64(sAddTradeVol);
                        }
                        else
                        {
                            if (stTradeData.nMHighPrice[i, nTimeCount] < Convert.ToInt32(sNowPrice))
                            {
                                stTradeData.nMHighPrice[i, nTimeCount] = Convert.ToInt32(sNowPrice);
                                //stTradeData.nMHighTime[i, nTimeCount] = nSecond;
                            }

                            if (stTradeData.nMLowPrice[i, nTimeCount] > Convert.ToInt32(sNowPrice))
                            {
                                stTradeData.nMLowPrice[i, nTimeCount] = Convert.ToInt32(sNowPrice);
                                //stTradeData.nMLowTime[i, nTimeCount] = nSecond;
                            }

                            stTradeData.nMEndPrice[i, nTimeCount] = Convert.ToInt32(sNowPrice);
                            stTradeData.lMTradVol[i, nTimeCount] += Convert.ToInt64(sNowTradeVol);
                            stTradeData.lMTradVolAll[i, nTimeCount] = Convert.ToInt64(sAddTradeVol);
                        }

                        if(nNowTime > 915 && stTradeData.nMTime[i, nTimeCount-3] != 0 && stTradeData.nState[i] != 6)
                        {
                            int n3Per = (int)(stTradeData.nClosePrice[i] * 0.03);

                            if(stTradeData.nMHighPrice[i, nTimeCount - 3] - stTradeData.nMLowPrice[i, nTimeCount - 3] < n3Per)
                            {
                                if(stTradeData.nMStartPrice[i, nTimeCount - 2] < stTradeData.nMEndPrice[i, nTimeCount - 2] && stTradeData.nMHighPrice[i, nTimeCount - 2] - stTradeData.nMLowPrice[i, nTimeCount - 2] > n3Per && stTradeData.lMTradVol[i, nTimeCount - 3] * 2 < stTradeData.lMTradVol[i, nTimeCount - 2])
                                {
                                    if(stTradeData.nMEndPrice[i, nTimeCount - 2] > stTradeData.nMEndPrice[i, nTimeCount - 1] && (stTradeData.nMStartPrice[i, nTimeCount - 2] + stTradeData.nMEndPrice[i, nTimeCount - 2]) / 2 < stTradeData.nMEndPrice[i, nTimeCount - 1])
                                    {
                                        stTradeData.nMTime[i, nTimeCount - 3] = 0;
                                        stTradeData.nState[i] = 10;
                                        LogManager.WriteLine("5분봉눌림(1) : " + stTradeData.sCode[i]);

                                        
                                    }
                                    else if(stTradeData.nMEndPrice[i, nTimeCount - 2] < stTradeData.nMEndPrice[i, nTimeCount - 1] && stTradeData.nMEndPrice[i, nTimeCount - 1] - stTradeData.nMEndPrice[i, nTimeCount - 2] < n3Per / 2)
                                    {
                                        stTradeData.nMTime[i, nTimeCount - 3] = 0;
                                        LogManager.WriteLine("5분봉눌림(2) : " + stTradeData.sCode[i]);
                                    }
                                }
                            }
                        }

                        if(stTradeData.nState[i] == 10)
                        {
                            if (m_nTradeCount < 4 && nNowTime < 1200 && stTradeData.nNowPrice[i] < stTradeData.nClosePrice[i] + (stTradeData.nClosePrice[i] * 0.25))
                            {
                                int nQty = 1;

                                if (m_nTradeCount < 0)
                                {
                                    m_nTradeCount = 0;
                                }

                                if (stTradeData.nNowPrice[i] > 0)
                                {
                                    nQty = 50000 / stTradeData.nNowPrice[i];
                                }

                                int lRet = 10;

                                stTradeData.nBuyQty[i] = 0;
                                stTradeData.nBuyPrice[i] = stTradeData.nNowPrice[i];

                                if ((nMinute % 5 > 0 && stTradeData.nMStartPrice[i, nTimeCount] < stTradeData.nNowPrice[i]) || stTradeData.nMHighPrice[i, nTimeCount - 2] < stTradeData.nNowPrice[i])
                                {
                                    if (nNowTime < 1000)
                                    {
                                        nQty = nQty * 2;
                                        m_nTradeCount++;
                                    }

                                    lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "03", 0, "");
                                }
                                else if (nMinute % 5 > 0 && stTradeData.nMStartPrice[i, nTimeCount] > stTradeData.nNowPrice[i])
                                {
                                    lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "00", stTradeData.nMLowPrice[i, nTimeCount - 2], "");
                                }

                                /*
                                if (nNowTime < 1100)
                                {
                                    lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "03", 0, "");
                                }
                                else
                                {
                                    lRet = SendOrder(stTradeData.sCode[i], nQty, 1, "00", stTradeData.nMLowPrice[i, nTimeCount - 2], "");
                                }
                                */

                                if (lRet == 0)
                                {
                                    stTradeData.nState[i] = 7;
                                    stTradeData.nOrderQty[i] = nQty;
                                    stTradeData.nBuyTime[i] = nNowTime;

                                    int sellTime = nNowTime + 10;

                                    if (sellTime % 100 > 60)
                                    {
                                        sellTime = sellTime + 40;
                                    }

                                    stTradeData.nSellTime[i] = sellTime;

                                    m_nTradeCount++;
                                    //LogManager.WriteLine("매수 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i]);
                                }
                            }
                        }
                        else if (stTradeData.nBuyTime[i] + 10 < nNowTime && stTradeData.nBuyQty[i] == 0 && stTradeData.nState[i] == 7)
                        {
                            LogManager.WriteLine("매수취소 :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i]);

                            int lRet = SendOrder(stTradeData.sCode[i], stTradeData.nOrderQty[i], 3, "00", 0, stTradeData.sOrderNo[i]);

                            if (lRet == 0)
                            {
                                stTradeData.nState[i] = 6;

                                stTradeData.bSellSignal[i] = false;
                                stTradeData.nBuyQty[i] = 0;
                                stTradeData.nBuyPrice[i] = 0;
                                stTradeData.nBuyTime[i] = 0;
                                stTradeData.nSellTime[i] = nNowTime;

                                m_nTradeCount--;
                            }
                        }
                        else if (stTradeData.nOrderQty[i] == stTradeData.nBuyQty[i] && stTradeData.nState[i] == 7)
                        {
                            int nSellPrice = 0;

                            if(stTradeData.nBuyTime[i] < 1000)
                            {
                                nSellPrice = stTradeData.nBuyPrice[i] + (int)(stTradeData.nBuyPrice[i] * 0.04);
                            }
                            else if (stTradeData.nBuyTime[i] < 1100)
                            {
                                nSellPrice = stTradeData.nBuyPrice[i] + (int)(stTradeData.nBuyPrice[i] * 0.025);
                            }
                            else if (stTradeData.nBuyTime[i] < 1200)
                            {
                                nSellPrice = stTradeData.nBuyPrice[i] + (int)(stTradeData.nBuyPrice[i] * 0.015);
                            }

                            if (nSellPrice >= 1000 && nSellPrice < 5000)
                                nSellPrice = nSellPrice - (nSellPrice % 10);
                            else if (nSellPrice >= 5000 && nSellPrice < 10000)
                                nSellPrice = nSellPrice - (nSellPrice % 20);
                            else if (nSellPrice >= 10000 && nSellPrice < 50000)
                                nSellPrice = nSellPrice - (nSellPrice % 100);
                            else if (nSellPrice >= 50000 && nSellPrice < 100000)
                                nSellPrice = nSellPrice - (nSellPrice % 200);
                            else if (nSellPrice >= 100000 && nSellPrice < 500000)
                                nSellPrice = nSellPrice - (nSellPrice % 1000);

                            int nQty = stTradeData.nBuyQty[i];

                            int lRet = SendOrder(stTradeData.sCode[i], nQty, 2, "00", nSellPrice, "");
                            //int lRet = SendOrder(stTradeData.sCode[i], nQty, 2, "07", 0, "");

                            if (lRet == 0)
                            {
                                stTradeData.nState[i] = 8;
                            }

                            /*
                            if (stTradeData.nMLowPrice[i, nTimeCount - 2] <= stTradeData.nMLowPrice[i, nTimeCount - 1] || stTradeData.nBuyPrice[i] < stTradeData.nNowPrice[i])
                            {
                                int sellTime = nNowTime + 5;

                                if (sellTime % 100 > 60)
                                {
                                    sellTime = sellTime + 40;
                                }

                                stTradeData.nSellTime[i] = sellTime;
                            }
                            else
                            {
                                LogManager.WriteLine("매도(Status7) :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + stTradeData.nBuyPrice[i].ToString() + "\t" + stTradeData.nNowPrice[i].ToString());

                                int nQty = stTradeData.nBuyQty[i];

                                //int lRet = SendOrder(stTradeData.sCode[i], nQty, 2, "00", nSellPrice, "");
                                int lRet = SendOrder(stTradeData.sCode[i], nQty, 2, "07", 0, "");

                                if (lRet == 0)
                                {
                                    stTradeData.nState[i] = 8;

                                    int sellTime = nNowTime + 5;

                                    if (sellTime % 100 > 60)
                                    {
                                        sellTime = sellTime + 40;
                                    }

                                    stTradeData.nSellTime[i] = sellTime;
                                }
                            }
                            */
                        }
                        /*
                        else if (stTradeData.nSellTime[i] <= nNowTime && stTradeData.nState[i] == 8)
                        {
                            int nNowPrice = Convert.ToInt32(stTradeData.nNowPrice[i]);

                            if (stTradeData.nSellTime[i] + 1 < nNowTime)
                            {
                                LogManager.WriteLine("정정(Status8) :\t" + stTradeData.sCode[i] + "\t" + stTradeData.sName[i] + "\t" + stTradeData.nNowPrice[i].ToString());
                                int lRet = SendOrder(stTradeData.sCode[i], stTradeData.nOrderQty[i], 3, "00", 0, stTradeData.sOrderNo[i]);
                                System.Threading.Thread.Sleep(2000);

                                lRet = SendOrder(stTradeData.sCode[i], stTradeData.nOrderQty[i], 2, "07", 0, "");

                                if (lRet == 0)
                                {
                                    int sellTime = nNowTime + 5;

                                    if (sellTime % 100 > 60)
                                    {
                                        sellTime = sellTime + 40;
                                    }

                                    stTradeData.nSellTime[i] = sellTime;
                                }
                            }
                        }
                        */

                        if (stTradeData.nPivot[i] == 0 && m_bNextDayChcek == true)
                        {
                            axKHOpenAPI.SetInputValue("종목코드", stTradeData.sCode[i]);
                            axKHOpenAPI.SetInputValue("기준일자", System.DateTime.Now.ToString("yyyyMMdd"));
                            axKHOpenAPI.SetInputValue("수정주가구분", "1");

                            m_bNextDayChcek = false;
                            int nRet = axKHOpenAPI.CommRqData("주식일봉차트조회", "OPT10081", 0, GetScrNum());
                        }

                        if(nNowTime > 920 && stTradeData.nNowPrice[i] < stTradeData.nClosePrice[i])
                        {
                            axKHOpenAPI.SetRealRemove("ALL", stTradeData.sCode[i]);  // 모든 화면에서 실시간 해지
                        }

                        if(stTradeData.nPivot[i] > 0 && stTradeData.nPivot[i] < stTradeData.nNowPrice[i] && stTradeData.nState[i] < 32)
                        {
                            stTradeData.nState[i] = 32;

                            nMinute = nMinute - (nMinute % 5) + 5;
                            if (nMinute + 10 >= 60)
                            {
                                stTradeData.nCheckTime[i] = nHour * 100 + 100 + (nMinute + 10 - 60);
                            }
                            else
                            {
                                stTradeData.nCheckTime[i] = nHour * 100 + nMinute + 10;
                            }
                            LogManager.WriteLine("피봇2차저항 돌파 : " + stTradeData.sCode[i] + " 피봇 : " + stTradeData.nPivot[i].ToString() + " 현재가 : " + stTradeData.nNowPrice[i].ToString());
                        }

                        if(stTradeData.nCheckTime[i] != 0 && stTradeData.nCheckTime[i] <= nNowTime)
                        {
                            if (stTradeData.nMStartPrice[i, 0] == 0 && m_bNextMinChcek == true)
                            {
                                axKHOpenAPI.SetInputValue("종목코드", stTradeData.sCode[i]);
                                axKHOpenAPI.SetInputValue("기준일자", System.DateTime.Now.ToString("yyyyMMdd"));
                                axKHOpenAPI.SetInputValue("수정주가구분", "1");
                                axKHOpenAPI.SetInputValue("틱범위", "5");

                                m_bNextMinChcek = false;
                                int nRet = axKHOpenAPI.CommRqData("주식분봉차트조회", "OPT10080", 0, GetScrNum());
                                LogManager.WriteLine("주식분봉차트조회 : " + stTradeData.sCode[i]);
                            }
                            
                            if(stTradeData.nState[i] == 32 && stTradeData.nNowPrice[i] < stTradeData.nPivot[i])
                            {
                                stTradeData.nState[i] = 33;
                                LogManager.WriteLine("피봇2차저항 보다 하락 : " + stTradeData.sCode[i]);
                            }

                            if (stTradeData.nState[i] == 33 && stTradeData.nNowPrice[i] > stTradeData.nPivot[i])
                            {
                                stTradeData.nState[i] = 34;

                                int nSellPrice = stTradeData.nPivot[i];

                                if (nSellPrice >= 1000 && nSellPrice < 5000)
                                    stTradeData.nPivotBuyPrice[i] = nSellPrice - 10;
                                else if (nSellPrice >= 5000 && nSellPrice < 10000)
                                    stTradeData.nPivotBuyPrice[i] = nSellPrice - 20;
                                else if (nSellPrice >= 10000 && nSellPrice < 50000)
                                    stTradeData.nPivotBuyPrice[i] = nSellPrice - 100;
                                else if (nSellPrice >= 50000 && nSellPrice < 100000)
                                    stTradeData.nPivotBuyPrice[i] = nSellPrice - 200;
                                else if (nSellPrice >= 100000 && nSellPrice < 500000)
                                    stTradeData.nPivotBuyPrice[i] = nSellPrice - 1000;

                                LogManager.WriteLine("피봇2차저항 보다 상승 : " + stTradeData.sCode[i] + " 매수 : " + stTradeData.nPivotBuyPrice[i].ToString());
                            }

                            if (stTradeData.nState[i] == 34 && stTradeData.nNowPrice[i] <= stTradeData.nPivotBuyPrice[i])
                            {
                                stTradeData.nState[i] = 35;
                                LogManager.WriteLine("매수체결 : " + stTradeData.sCode[i] + " 매수가격 : " + stTradeData.nPivotBuyPrice[i].ToString() + " 현재가격 : " + stTradeData.nNowPrice[i].ToString());
                                axKHOpenAPI.SetRealRemove("ALL", stTradeData.sCode[i]);  // 모든 화면에서 실시간 해지
                            }
                        }

                        /*
                        if (stTradeData.nHighPrice[i] <= stTradeData.nNowPrice[i] && nHour * 100 + nMinute < 905)
                        {
                            stTradeData.nHighPrice[i] = stTradeData.nNowPrice[i];
                            stTradeData.nHighTime[i] = (nHour * 10000) + (nMinute * 100) + nSecond;
                        }
                        else if (stTradeData.nHighPrice[i] <= stTradeData.nNowPrice[i] && (stTradeData.nHighTime[i] == 0 || stTradeData.nHighTime[i] + 10 > nHour * 100 + nMinute))
                        {
                            stTradeData.nHighPrice[i] = stTradeData.nNowPrice[i];
                            stTradeData.nLowPrice[i] = stTradeData.nNowPrice[i];

                            if (nMinute > 50)
                            {
                                stTradeData.nHighTime[i] = (nHour + 1) * 100;
                                stTradeData.nLowTime[i] = (nHour + 1) * 100;
                            }
                            else
                            {
                                stTradeData.nHighTime[i] = nHour * 100 + nMinute;
                                stTradeData.nLowTime[i] = nHour * 100 + nMinute;
                            }
                        }

                        if (stTradeData.nLowPrice[i] >= stTradeData.nNowPrice[i] && (stTradeData.nLowPrice[i] == 0 || stTradeData.nLowTime[i] + 10 > nHour * 100 + nMinute))
                        {
                            stTradeData.nLowPrice[i] = stTradeData.nNowPrice[i];

                            if (nMinute > 50)
                            {
                                stTradeData.nLowTime[i] = (nHour + 1) * 100;
                            }
                            else
                            {
                                stTradeData.nLowTime[i] = nHour * 100 + nMinute;
                            }
                        }

                        if (stTradeData.nStandardPrice[i] < stTradeData.nNowPrice[i])
                        {
                            stTradeData.nStandardPrice[i] = stTradeData.nNowPrice[i];
                            stTradeData.nStandardTime[i] = (nHour * 10000) + (nMinute * 100) + nSecond;
                        }


                        if(stTradeData.nMTime[i,stTradeData.nMCount[i]] == 0)
                        {
                            stTradeData.nMTime[i, stTradeData.nMCount[i]] = nHour * 100 + nMinute;
                            stTradeData.nMStartPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMEndPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMHighPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMHighTime[i, stTradeData.nMCount[i]] = nSecond;
                            stTradeData.nMLowPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMLowTime[i, stTradeData.nMCount[i]] = nSecond;
                            stTradeData.lMTradVol[i, stTradeData.nMCount[i]] = Convert.ToInt64(sNowTradeVol);
                            stTradeData.lMTradVolAll[i, stTradeData.nMCount[i]] = Convert.ToInt64(sNowTradeVol);
                        }
                        else if (stTradeData.nMTime[i, stTradeData.nMCount[i]] < nHour * 100 + nMinute)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                if (j < 4)
                                {
                                    stTradeData.n5MinutePrice[i, j] = stTradeData.n5MinutePrice[i, j + 1];
                                }

                                stTradeData.n10MinutePrice[i, j] = stTradeData.n10MinutePrice[i, j + 1];
                            }

                            stTradeData.n5MinutePrice[i, 4] = stTradeData.nMEndPrice[i, stTradeData.nMCount[i]];
                            stTradeData.n10MinutePrice[i, 9] = stTradeData.nMEndPrice[i, stTradeData.nMCount[i]];

                            int n5Count = 0;
                            int n10Count = 0;
                            int n5Price = 0;
                            int n10Price = 0;

                            for (int j = 0; j < 10; j++)
                            {
                                if (j < 5)
                                {
                                    if(stTradeData.n5MinutePrice[i, j] > 0)
                                    {
                                        n5Price += stTradeData.n5MinutePrice[i, j];
                                        n5Count++;
                                    }
                                }

                                if (stTradeData.n10MinutePrice[i, j] > 0)
                                {
                                    n10Price += stTradeData.n10MinutePrice[i, j];
                                    n10Count++;
                                }
                            }

                            stTradeData.n5MinuteAverage[i] = n5Price / n5Count;
                            stTradeData.n10MinuteAverage[i] = n10Price / n10Count;

                            if(m_nAverageHigh < stTradeData.nMHighPrice[i, stTradeData.nMCount[i]])
                            {
                                m_nAverageHigh = stTradeData.nMHighPrice[i, stTradeData.nMCount[i]];
                            }

                            if(stTradeData.n5MinuteAverage[i] > stTradeData.n10MinuteAverage[i] && stTradeData.nAverageStatus[i] != 1)       //UpAverage
                            {
                                stTradeData.nAverageStatus[i] = 1;

                                stTradeData.nUpAverageEnd1[i] = stTradeData.nUpAverageEnd2[i];
                                stTradeData.nUpAverageEnd2[i] = stTradeData.nMEndPrice[i, stTradeData.nMCount[i]];

                                stTradeData.nUpAverageHigh1[i] = stTradeData.nUpAverageHigh2[i];
                                stTradeData.nUpAverageHigh2[i] = m_nAverageHigh;
                                m_nAverageHigh = 0;
                            }
                            else if (stTradeData.n5MinuteAverage[i] < stTradeData.n10MinuteAverage[i] && stTradeData.nAverageStatus[i] != 2)       //DownAverage
                            {
                                stTradeData.nAverageStatus[i] = 2;

                                stTradeData.nDownAverageEnd1[i] = stTradeData.nDownAverageEnd2[i];
                                stTradeData.nDownAverageEnd2[i] = stTradeData.nMEndPrice[i, stTradeData.nMCount[i]];

                                stTradeData.nDownAverageHigh1[i] = stTradeData.nDownAverageHigh2[i];
                                stTradeData.nDownAverageHigh2[i] = m_nAverageHigh;
                                m_nAverageHigh = 0;
                            }

                            stTradeData.nMCount[i]++;
                            stTradeData.nMTime[i, stTradeData.nMCount[i]] = nHour * 100 + nMinute;
                            stTradeData.nMStartPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMEndPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMHighPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMHighTime[i, stTradeData.nMCount[i]] = nSecond;
                            stTradeData.nMLowPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                            stTradeData.nMLowTime[i, stTradeData.nMCount[i]] = nSecond;
                            //stTradeData.lMTradVol[i, stTradeData.nMCount[i]] = Convert.ToInt64(sNowTradeVol) - stTradeData.lMTradVolAll[i, stTradeData.nMCount[i-1]];
                            stTradeData.lMTradVolAll[i, stTradeData.nMCount[i]] = Convert.ToInt64(sNowTradeVol);
                        }
                        else
                        {
                            stTradeData.nMEndPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);

                            if(stTradeData.nMHighPrice[i, stTradeData.nMCount[i]] < Convert.ToInt32(sNowPrice))
                            {
                                stTradeData.nMHighPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                                stTradeData.nMHighTime[i, stTradeData.nMCount[i]] = nSecond;
                            }
                            else if (stTradeData.nMLowPrice[i, stTradeData.nMCount[i]] > Convert.ToInt32(sNowPrice))
                            {
                                stTradeData.nMLowPrice[i, stTradeData.nMCount[i]] = Convert.ToInt32(sNowPrice);
                                stTradeData.nMLowTime[i, stTradeData.nMCount[i]] = nSecond;
                            }

                            if(stTradeData.nMCount[i] == 0)
                            {
                                stTradeData.lMTradVol[i, stTradeData.nMCount[i]] = Convert.ToInt64(sNowTradeVol);
                                stTradeData.lMTradVolAll[i, stTradeData.nMCount[i]] = Convert.ToInt64(sNowTradeVol);
                            }
                            else
                            {
                                //stTradeData.lMTradVol[i, stTradeData.nMCount[i]] = Convert.ToInt64(sNowTradeVol) - stTradeData.lMTradVolAll[i, stTradeData.nMCount[i - 1]];
                                stTradeData.lMTradVolAll[i, stTradeData.nMCount[i]] = Convert.ToInt64(sNowTradeVol);
                            }
                        }

                        */
                    }
                }
            }

            /*
            Logger(Log.실시간, "종목코드 : {0} | RealType : {1} | RealData : {2}",
                e.sRealKey, e.sRealType, e.sRealData);

            if( e.sRealType == "주식시세" )
            {
                Logger(Log.실시간, "종목코드 : {0} | 현재가 : {1:C} | 등락율 : {2} | 누적거래량 : {3:N0} ",
                        e.sRealKey,
                        Int32.Parse(axKHOpenAPI.GetCommRealData(e.sRealType, 10).Trim()),
                        axKHOpenAPI.GetCommRealData(e.sRealType, 12).Trim(),
                        Int32.Parse(axKHOpenAPI.GetCommRealData(e.sRealType, 13).Trim()));
            }
            */
        }

        private void 계좌조회ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbl아이디.Text = axKHOpenAPI.GetLoginInfo("USER_ID");
            lbl이름.Text = axKHOpenAPI.GetLoginInfo("USER_NAME");

            string[] arr계좌 = axKHOpenAPI.GetLoginInfo("ACCNO").Split(';');

            cbo계좌.Items.AddRange(arr계좌);
            cbo계좌.SelectedIndex = 0;
        }

        private void 현재가ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axKHOpenAPI.SetInputValue("종목코드", txt종목코드.Text.Trim());

            int nRet = axKHOpenAPI.CommRqData("주식기본정보", "OPT10001", 0, GetScrNum());
            _scrNum++;

            if (Error.IsError(nRet))
            {
                Logger(Log.일반, "[OPT10001] : " + Error.GetErrorMessage());
            }
            else
            {
                Logger(Log.에러, "[OPT10001] : " + Error.GetErrorMessage());
            }
        }

        private void 일봉데이터ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = stTradeData.sCode[0].Trim();
            axKHOpenAPI.SetInputValue("종목코드", txt종목코드.Text.Trim());
            //axKHOpenAPI.SetInputValue("종목코드", code);
            axKHOpenAPI.SetInputValue("기준일자", System.DateTime.Now.ToString("yyyyMMdd"));
            axKHOpenAPI.SetInputValue("수정주가구분", "1");
            axKHOpenAPI.SetInputValue("틱범위", "1");

            int nRet = axKHOpenAPI.CommRqData("주식분봉차트조회", "OPT10080", 0, GetScrNum());
            //int nRet = axKHOpenAPI.CommRqData("주식일봉차트조회", "OPT10081", 0, GetScrNum());
            _scrNum++;

            if (Error.IsError(nRet))
            {
                Logger(Log.일반, "[OPT10081] : " + Error.GetErrorMessage());
            }
            else
            {
                Logger(Log.에러, "[OPT10081] : " + Error.GetErrorMessage());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // =================================================
            // 거래구분목록 지정
            for (int i = 0; i < 12; i++)
                cbo거래구분.Items.Add(KOACode.hogaGb[i].name);
            
            cbo거래구분.SelectedIndex = 0;


            // =================================================
            // 주문유형
            for(int i = 0; i < 5; i++)
                cbo매매구분.Items.Add(KOACode.orderType[i].name);

            cbo매매구분.SelectedIndex = 0;
        }

        private void txt주문종목코드_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void txt주문수량_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void txt주문가격_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void txt원주문번호_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void txt종목코드_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void txt조회날짜_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void btn주문_Click(object sender, EventArgs e)
        {
            // =================================================
            // 계좌번호 입력 여부 확인
            if( cbo계좌.Text.Length != 10 )
            {
                Logger(Log.에러, "계좌번호 10자리를 입력해 주세요");

                return;
            }

            // =================================================
            // 종목코드 입력 여부 확인
            if( txt주문종목코드.TextLength != 6 )
            {
                Logger(Log.에러, "종목코드 6자리를 입력해 주세요");

                return;
            }

            // =================================================
            // 주문수량 입력 여부 확인
            int n주문수량;

            if(txt주문수량.TextLength > 0)
            {
                n주문수량 = Int32.Parse(txt주문수량.Text.Trim());
            }
            else
            {
                Logger(Log.에러, "주문수량을 입력하지 않았습니다");
                
                return;
            }

            if( n주문수량 < 1 )
            {
                Logger(Log.에러, "주문수량이 1보다 작습니다");
                
                return;
            }

            // =================================================
            // 거래구분 취득
            // 0:지정가, 3:시장가, 5:조건부지정가, 6:최유리지정가, 7:최우선지정가,
            // 10:지정가IOC, 13:시장가IOC, 16:최유리IOC, 20:지정가FOK, 23:시장가FOK,
            // 26:최유리FOK, 61:장개시전시간외, 62:시간외단일가매매, 81:시간외종가
        
            string s거래구분;
            s거래구분 = KOACode.hogaGb[cbo거래구분.SelectedIndex].code;

            // =================================================
            // 주문가격 입력 여부

            int n주문가격 = 0;

            if( txt주문가격.TextLength > 0 )
            {
                n주문가격 = Int32.Parse(txt주문가격.Text.Trim());
            }

            if (s거래구분 == "3" || s거래구분 == "13" || s거래구분 == "23" && n주문가격 < 1)
            {
                Logger(Log.에러, "주문가격이 1보다 작습니다");
            }

            // =================================================
            // 매매구분 취득
            // (1:신규매수, 2:신규매도 3:매수취소, 
            // 4:매도취소, 5:매수정정, 6:매도정정)

            int n매매구분;
            n매매구분 = KOACode.orderType[cbo매매구분.SelectedIndex].code;

            // =================================================
            // 원주문번호 입력 여부

            if( n매매구분 > 2 && txt원주문번호.TextLength < 1 )
            {
                Logger(Log.에러, "원주문번호를 입력해주세요");
            }


            // =================================================
            // 주식주문
            int lRet;

            lRet = axKHOpenAPI.SendOrder("주식주문", GetScrNum(), cbo계좌.Text.Trim(), 
                                        n매매구분, txt주문종목코드.Text.Trim(), n주문수량, 
                                        n주문가격, s거래구분, txt원주문번호.Text.Trim());

            if( lRet == 0 )
            {
                Logger(Log.일반, "주문이 전송 되었습니다");
            }
            else
            {
                Logger(Log.에러, "주문이 전송 실패 하였습니다. [에러] : " + lRet);
            }
        }

        private void 주문ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btn주문_Click(sender, e);
        }

        private void 조건식로컬저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int lRet;

            lRet = axKHOpenAPI.GetConditionLoad();

            if (lRet == 1)
            {
                Logger(Log.일반, "조건식 저장이 성공 되었습니다");
            }
            else
            {
                Logger(Log.에러, "조건식 저장이 실패 하였습니다");
            }
        }

        private void axKHOpenAPI_OnReceiveConditionVer(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveConditionVerEvent e)
        {
            if( e.lRet == 1 )
            {
                Logger(Log.일반, "[이벤트] 조건식 저장 성공");

                string strConList;

                strConList = axKHOpenAPI.GetConditionNameList().Trim();

                Logger(Log.조회, strConList);

                // 분리된 문자 배열 저장
                string[] spConList = strConList.Split(';');

                // ComboBox 출력
                for (int i = 0; i < spConList.Length; i++)
                {
                    if (spConList[i].Trim().Length >= 2)
                    {
                        cbo조건식.Items.Add(spConList[i]);
                    }
                }

                cbo조건식.SelectedIndex = 0;
            }
            else
            {
                Logger(Log.에러, "[이벤트] 조건식 저장 실패 : " + e.sMsg);
            }

        }

        private void 조건명리스트호출ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strConList;

            strConList = axKHOpenAPI.GetConditionNameList().Trim();

            Logger(Log.조회, strConList);

            // 분리된 문자 배열 저장
            string[] spConList = strConList.Split(';');

            // ComboBox 출력
            for(int i = 0; i < spConList.Length; i++)
            {
                if(spConList[i].Trim().Length >= 2)
                {
                    cbo조건식.Items.Add(spConList[i]);
                    /*
                    string[] spCon = spConList[i].Split('^');
                    int nIndex = Int32.Parse(spCon[0]);
                    string strConditionName = spCon[1];
                    cbo조건식.Items.Add(strConditionName);
                    */
                }
            }

            cbo조건식.SelectedIndex = 0;
        }

        private void btn_조건일반조회_Click(object sender, EventArgs e)
        {
            string[] spCon = cbo조건식.Text.Split('^');
            int nCondNumber = Int32.Parse(spCon[0]);    // 조건번호
            string spCondName = spCon[1];               // 조건식 이름

            m_bLiveCheckThread = true;
            System.Threading.Thread LiveCheckThread = new System.Threading.Thread(new System.Threading.ThreadStart(CheckLiveStock));
            LiveCheckThread.Start();
            return;

            int lRet = axKHOpenAPI.SendCondition(GetScrNum(), spCondName, nCondNumber, 0);
            
            if (lRet == 1)
            {
                Logger(Log.일반, "조건식 일반 조회 실행이 성공 되었습니다");
            }
            else
            {
                Logger(Log.에러, "조건식 일반 조회 실행이 실패 하였습니다");
            }
        }

        private void btn조건실시간조회_Click(object sender, EventArgs e)
        {
            /*
            string[] spCon = cbo조건식.Text.Split('^');
            m_nCondNumber = Int32.Parse(spCon[0]);    // 조건번호
            m_spCondName = spCon[1];               // 조건식 이름

            m_bLiveCheckThread = true;
            System.Threading.Thread LiveCheckThread = new System.Threading.Thread(new System.Threading.ThreadStart(CheckLiveStock));
            LiveCheckThread.Start();
            return;
            */

            string[] spCon = cbo조건식.Text.Split('^');
            int nCondNumber = Int32.Parse(spCon[0]);    // 조건번호
            string spCondName = spCon[1];               // 조건식 이름
            string strScrNum = GetScrNum();
            int lRet = axKHOpenAPI.SendCondition(strScrNum, spCondName, nCondNumber, 1);

            if (lRet == 1)
            {
                _strRealConScrNum = strScrNum;
                _strRealConName = cbo조건식.Text;
                _nIndex = cbo조건식.SelectedIndex;

                Logger(Log.일반, "조건식 실시간 조회 실행이 성공 되었습니다");
            }
            else
            {
                Logger(Log.에러, "조건식 실시간 조회 실행이 실패 하였습니다");
            }
        }

        private void axKHOpenAPI_OnReceiveRealCondition(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
        {
            //Logger(Log.실시간, "========= 조건조회 실시간 편입/이탈 ==========");
            //Logger(Log.실시간, "[종목코드] : " + e.sTrCode);
            //Logger(Log.실시간, "[실시간타입] : " + e.strType);
            //Logger(Log.실시간, "[조건명] : " + e.strConditionName);
            //Logger(Log.실시간, "[조건명 인덱스] : " + e.strConditionIndex);

            if(!e.strConditionName.Contains("(단타)"))
            {
                //LogManager.WriteLine("[종목코드] : " + e.sTrCode + "[조건명] : " + e.strConditionName + "[실시간타입] : " + e.strType);
            }

            if (e.sTrCode != "" && e.strType == "I")
                AddTradeList(e.sTrCode);

            // 자동주문 로직
            if (_bRealTrade && e.strType == "I")
            {
                // 해당 종목 1주 시장가 주문
                // =================================================

                // 계좌번호 입력 여부 확인
                if (cbo계좌.Text.Length != 10)
                {
                    Logger(Log.에러, "계좌번호 10자리를 입력해 주세요");

                    return;
                }

                // =================================================
                // 주식주문
                int lRet;

                lRet = axKHOpenAPI.SendOrder("주식주문", 
                                            GetScrNum(), 
                                            cbo계좌.Text.Trim(),
                                            1,      // 매매구분
                                            e.sTrCode.Trim(),   // 종목코드
                                            1,      // 주문수량
                                            1,      // 주문가격 
                                            "03",    // 거래구분 (시장가)
                                            "0");    // 원주문 번호

                if (lRet == 0)
                {
                    Logger(Log.일반, "주문이 전송 되었습니다");
                }
                else
                {
                    Logger(Log.에러, "주문이 전송 실패 하였습니다. [에러] : " + lRet);
                }
            }
        }

        private void axKHOpenAPI_OnReceiveTrCondition(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
        {
            Logger(Log.조회, "[화면번호] : " + e.sScrNo);
            Logger(Log.조회, "[종목리스트] : " + e.strCodeList);
            Logger(Log.조회, "[조건명] : " + e.strConditionName);
            Logger(Log.조회, "[조건명 인덱스 ] : " + e.nIndex.ToString());
            Logger(Log.조회, "[연속조회] : " + e.nNext.ToString());

            if (e.strCodeList != "")
                AddTradeList(e.strCodeList);
        }

        private void btn_조건실시간중지_Click(object sender, EventArgs e)
        {
            if( _strRealConScrNum != "0000" &&
                _strRealConName != "0000" )
            {
                axKHOpenAPI.SendConditionStop(_strRealConScrNum, _strRealConName, _nIndex);

                Logger(Log.실시간, "========= 실시간 조건 조회 중단 ==========");
                Logger(Log.실시간, "[화면번호] : " + _strRealConScrNum + " [조건명] : " + _strRealConName);
            }
        }

        private void btn실시간등록_Click(object sender, EventArgs e)
        {
            long lRet;

            lRet = axKHOpenAPI.SetRealReg(  GetScrNum(),              // 화면번호
                                            txt실시간종목코드.Text,    // 종콕코드 리스트
                                            "9001;10",  // FID번호
                                            "0");       // 0 : 마지막에 등록한 종목만 실시간

            if (lRet == 0)
            {
                Logger(Log.일반, "실시간 등록이 실행되었습니다");
            }
            else
            {
                Logger(Log.에러, "실시간 등록이 실패하였습니다");
            }
        }

        private void btn실시간해제_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 200; i++)
            {
                if (stTradeData.sCode[i] != "")
                {
                    axKHOpenAPI.SetInputValue("종목코드", stTradeData.sCode[i]);
                    axKHOpenAPI.SetInputValue("기준일자", System.DateTime.Now.ToString("yyyyMMdd"));
                    axKHOpenAPI.SetInputValue("수정주가구분", "1");
                    axKHOpenAPI.SetInputValue("틱범위", "5");

                    m_bNextDayChcek = false;
                    int nRet = axKHOpenAPI.CommRqData("주식분봉차트조회", "OPT10080", 0, GetScrNum());

                    /*
                    while (true)
                    {
                        if (m_bNextDayChcek == true)
                            break;

                        System.Threading.Thread.Sleep(2000);
                    }
                    */
                    System.Threading.Thread.Sleep(4000);
                }
            }

            return;

            axKHOpenAPI.SetRealRemove(  "ALL",     // 화면번호
                                        "ALL");    // 실시간 해제할 종목코드

            Logger(Log.실시간, "======= 실시간 해제 실행 ========");
        }

        private void btn자동주문_Click(object sender, EventArgs e)
        {
            //string aa = axKHOpenAPI.GetMasterCodeName("019550");
            //aa = axKHOpenAPI.GetMasterLastPrice("019550");
            //string sNowPrice = axKHOpenAPI.GetCommRealData("019550", 10).Trim();

            //axKHOpenAPI.SetInputValue("계좌번호", "5198658610");
            //axKHOpenAPI.SetInputValue("비밀번호", "");
            //axKHOpenAPI.SetInputValue("상장폐지조회구분", "0");
            //axKHOpenAPI.SetInputValue("비밀번호입력매체구분", "00");
            //axKHOpenAPI.SetInputValue("종목번호", "008700");
            //axKHOpenAPI.SetInputValue("매수가격", "3600");

            //axKHOpenAPI.CommRqData("계좌평가현황요청", "opw00004", 0, GetScrNum());
            //axKHOpenAPI.CommRqData("체결잔고요청", "opw00005", 0, GetScrNum());

            //axKHOpenAPI.SetInputValue("종목코드", "019550");
            //int nRet = axKHOpenAPI.CommRqData("주식기본정보", "OPT10001", 0, GetScrNum());

            //AddTradeList("032820;010820;101390;078130;001510;002100;039610;054780;008350;");

            axKHOpenAPI.SetInputValue("계좌번호", "5198658610");
            axKHOpenAPI.SetInputValue("비밀번호", "");
            axKHOpenAPI.SetInputValue("상장폐지조회구분", "0");
            axKHOpenAPI.SetInputValue("비밀번호입력매체구분", "00");

            axKHOpenAPI.CommRqData("계좌평가현황요청", "opw00004", 0, GetScrNum());

            //System.Threading.Thread TradeThread = new System.Threading.Thread(new System.Threading.ThreadStart(TradeDataCheck));
            //TradeThread.Start();

            return;

            if (_bRealTrade)
            {
                btn자동주문.Text = "자동주문 시작";
                _bRealTrade = false;
                Logger(Log.일반, "======= 자동 주문 중단 ========");
            }
            else
            {
                btn자동주문.Text = "자동주문 중단";
                _bRealTrade = true;
                Logger(Log.일반, "======= 자동 주문 실행 ========");
            }
        }

        private void btnSaleValue_Click(object sender, EventArgs e)
        {
            if(m_bSale == true)
            {
                m_bSale = false;
                btnSaleValue.Text = "매수시작";
            }
            else
            {
                m_bSale = true;
                btnSaleValue.Text = "매수중지";
            }
        }

        private void btnReadLog_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.DefaultExt = "txt";
            openFileDlg.Filter = "TXT Files(*.txt;*.log)|*.txt;*.log";
            openFileDlg.ShowDialog();

            string line;

            if (openFileDlg.FileName.Length > 0)
            {
                foreach (string filename in openFileDlg.FileNames)
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(filename);
                    while ((line = file.ReadLine()) != null)
                    {
                        if(filename.Contains(".txt"))
                        {
                            if (line.Contains("종목 :"))
                            {
                                string[] item = line.Split('\t');
                                AddTradeList(item[1] + ";", 10, 0, 0);
                            }
                        }
                        else
                        {
                            string[] item = line.Split('\t');
                            AddTradeList(item[0], 10, 0, 0);

                            axKHOpenAPI.SetInputValue("종목코드", item[0].Substring(0,6));
                            axKHOpenAPI.SetInputValue("기준일자", System.DateTime.Now.ToString("yyyyMMdd"));
                            axKHOpenAPI.SetInputValue("수정주가구분", "1");

                            m_bNextDayChcek = false;
                            int nRet = axKHOpenAPI.CommRqData("주식일봉차트조회", "OPT10081", 0, GetScrNum());

                            
                        }
                    }
                    file.Close();
                    Logger(Log.일반, "[로그 읽기 완료] " + filename);
                }
            }

            openFileDlg.Dispose();
        }

        private void btnSaveDB_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "TXT Files(*.txt;*.log)|*.txt;*.log";
            saveFileDlg.ShowDialog();

            if (saveFileDlg.FileName != "")
            {
                StreamWriter sw = new StreamWriter(saveFileDlg.FileName, true);
                for(int i = 0; i < 200; i++)
                {
                    if(stTradeData.sCode[i] != "")
                    {
                        sw.WriteLine(stTradeData.sCode[i] + ";\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0");
                    }
                }
                
                sw.Flush();
                sw.Close();
            }
            
        }

        private void btnCheckDayChart_Click(object sender, EventArgs e)
        {
            /*
            axKHOpenAPI.SetInputValue("종목코드", stTradeData.sCode[0]);
            axKHOpenAPI.SetInputValue("기준일자", System.DateTime.Now.ToString("yyyyMMdd"));
            axKHOpenAPI.SetInputValue("수정주가구분", "1");

            m_bNextDayChcek = false;
            int nRet = axKHOpenAPI.CommRqData("주식일봉차트조회", "OPT10081", 0, GetScrNum());

            return;
            */

            m_bNextDayChcek = true;
            System.Threading.Thread DayCheckThread = new System.Threading.Thread(new System.Threading.ThreadStart(CheckDayStock));
            DayCheckThread.Start(); 
        }

        public void CheckDayStock()
        {
            try
            {
                while (true)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        axKHOpenAPI.SetInputValue("종목코드", stTradeData.sCode[i]);
                        axKHOpenAPI.SetInputValue("기준일자", System.DateTime.Now.ToString("yyyyMMdd"));
                        axKHOpenAPI.SetInputValue("수정주가구분", "1");

                        m_bNextDayChcek = false;
                        int nRet = axKHOpenAPI.CommRqData("주식일봉차트조회", "OPT10081", 0, GetScrNum());
                        //_scrNum++;

                        while(true)
                        {
                            if (m_bNextDayChcek == true)
                                break;

                            System.Threading.Thread.Sleep(2000);
                        }
                    }

                    return;
                }
            }
            catch (Exception)
            {
                return;
                throw;
            }
        }
    }
}
