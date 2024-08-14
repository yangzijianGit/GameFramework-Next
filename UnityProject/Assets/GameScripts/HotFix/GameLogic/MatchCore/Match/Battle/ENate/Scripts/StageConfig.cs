/*
        author      :       yangzijian
        time        :       2019-12-26 17:29:06f
        function    :       read stageConfig.xml
*/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace ENate
{

    //元素参数
    public sealed class ElementParam
    {
        //元素名、限制数量、完成类型(1,2,3)、棋盘id号(1,2,3...)
        public string strName = string.Empty;
        public int nCount = 0;
        public int nType = 0;
        public int nBoard = 0;

        /** 构造函数 **/
        public ElementParam() { }
        public ElementParam(string name, int type, int board, int count = 0)
        {
            this.strName = name;
            this.nType = type;
            this.nBoard = board;
            this.nCount = count;
        }

        /** 清理 **/
        public void Clear()
        {
            this.strName = string.Empty;
            this.nCount = 0;
            this.nType = 0;
            this.nBoard = 0;
        }
    }

    //单元格参数
    public sealed class CellGridParam
    {
        //位置x、位置y
        public int nPosX = 0;
        public int nPosY = 0;
        //掉落器id
        public string strDropId = string.Empty;
        //格子方向
        public int intDirection = 6;
        //元素列表
        public List<string> m_listElementId = new List<string>();

        /** 构造函数 **/
        public CellGridParam() { }
        public CellGridParam(string name, int count) { }

        /** 清理 **/
        public void Clear()
        {
            this.nPosX = 0;
            this.nPosY = 0;
            this.strDropId = string.Empty;
            this.m_listElementId.Clear();
        }
    }

    //棋盘参数
    public sealed class BoardParam
    {
        //棋盘号(id)
        public int nId = 0;

        //棋盘尺寸(行、列)
        public int nRow = 0;
        public int nCol = 0;
        public bool m_bIsConnectLastChessBoard = false;
        public Direction m_eConnectDirection = Direction.Down;
        //格子列表
        public List<CellGridParam> m_listCellGrid = new List<CellGridParam>();

        /** 构造函数 **/
        public BoardParam() { }

        /** 清理 **/
        public void Clear()
        {
            this.nId = 0;
            this.nRow = 0;
            this.nCol = 0;
            foreach (var cell in this.m_listCellGrid)
            {
                cell.Clear();
            }
            this.m_listCellGrid.Clear();
        }
    }

    public class StageConfig
    {
        /** 头部参数 **/
        //创建方式(0自动，1手动)，创建时间0000-00-00 00:00:00，最近一次修改时间
        public int m_nCreateWay = 0;
        public string m_strCreateTime = string.Empty;
        public string m_strLastTime = string.Empty;

        /** 系统参数 **/
        //文件名、关卡名(ID)
        public string m_strFileName = string.Empty;
        public string m_strLevelName = string.Empty;

        /** 通关参数 **/
        //限制步数，限制时间，限制NPC，限制元素
        public int m_nStep = 0;
        public int m_nTime = 0;
        public int m_nNPC = 0;
        public List<ElementParam> m_listElement = new List<ElementParam>();

        /** 布局数据 **/
        //棋盘列表
        public List<BoardParam> m_listBoard = new List<BoardParam>();

        /** 辅助变量(不写入文件) **/
        //参数不完整类型、布局不完整类型(-1不完整错误，1不完整，0完整)
        public int m_nParamIncompleteType = 0;
        public int m_nLayoutIncompleteType = 0;
        //是否是新增关卡
        public bool m_bIsAddLevel = false;

        // public List<GroupParam> m_arrGroupParam = new List<GroupParam>();

        /** 构造 **/

        /** 清理 **/
        //读取数据
        public void Load(string xmlText)
        {
            if (null == xmlText)
            {
                return;
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlText);
            XmlNodeList rootNodes = xmlDocument.GetElementsByTagName("root");
            XmlNode tRootNode = rootNodes[0];
            XmlNodeList rootChild = tRootNode.ChildNodes;
            foreach (XmlNode rc in rootChild)
            {
                if (rc.Name == "game")
                {
                    XmlNodeList gameChild = rc.ChildNodes;
                    foreach (XmlNode gc in gameChild)
                    {
                        switch (gc.Name)
                        {
                            case "createWay":
                                {
                                    m_nCreateWay = int.Parse(gc.InnerText);
                                    break;
                                }
                            case "createTime":
                                {
                                    m_strCreateTime = gc.InnerText;
                                    break;
                                }
                            case "lastTime":
                                {
                                    m_strLastTime = gc.InnerText;
                                    break;
                                }
                            case "sysParam":
                                {
                                    XmlNodeList spList = gc.ChildNodes;
                                    foreach (XmlNode sp in spList)
                                    {
                                        switch (sp.Name)
                                        {
                                            case "levelId":
                                                {
                                                    m_strLevelName = sp.InnerText;
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
                            case "passParam":
                                {
                                    XmlNodeList ppList = gc.ChildNodes;
                                    foreach (XmlNode pp in ppList)
                                    {
                                        switch (pp.Name)
                                        {
                                            case "limitStep":
                                                {
                                                    m_nStep = int.Parse(pp.InnerText);
                                                    break;
                                                }
                                            case "limitNPC":
                                                {
                                                    m_nNPC = int.Parse(pp.InnerText);
                                                    break;
                                                }
                                            case "limitElement":
                                                {
                                                    XmlNodeList leList = pp.ChildNodes;
                                                    foreach (XmlNode le in leList)
                                                    {
                                                        switch (le.Name)
                                                        {
                                                            case "element":
                                                                {
                                                                    ElementParam objElementParam = new ElementParam();
                                                                    XmlNodeList eList = le.ChildNodes;
                                                                    foreach (XmlNode e in eList)
                                                                    {
                                                                        switch (e.Name)
                                                                        {
                                                                            case "name":
                                                                                {
                                                                                    objElementParam.strName = e.InnerText;
                                                                                    break;
                                                                                }
                                                                            case "type":
                                                                                {
                                                                                    objElementParam.nType = int.Parse(e.InnerText);
                                                                                    break;
                                                                                }
                                                                            case "count":
                                                                                {
                                                                                    objElementParam.nCount = int.Parse(e.InnerText);
                                                                                    break;
                                                                                }
                                                                            case "boardId":
                                                                                {
                                                                                    objElementParam.nBoard = int.Parse(e.InnerText);
                                                                                    break;
                                                                                }
                                                                        }
                                                                    }
                                                                    m_listElement.Add(objElementParam);
                                                                    break;
                                                                }
                                                        }
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
                            case "groupNode":
                                {
                                    XmlNodeList bList = gc.ChildNodes;
                                    foreach (XmlNode b in bList)
                                    {
                                        switch (b.Name)
                                        {
                                            case "groupInfo":
                                                {
                                                    // GroupParam tGroupParam = new GroupParam();
                                                    // XmlNodeList bbList = b.ChildNodes;
                                                    // foreach (XmlNode bb in bbList)
                                                    // {
                                                    //     switch (bb.Name)
                                                    //     {
                                                    //         case "GroupId":
                                                    //             {
                                                    //                 tGroupParam.m_strGroupId = bb.InnerText;
                                                    //             }
                                                    //             break;
                                                    //         case "ElementId":
                                                    //             {
                                                    //                 tGroupParam.m_strElementId = bb.InnerText;
                                                    //             }
                                                    //             break;
                                                    //         case "DestroyType":
                                                    //             {
                                                    //                 tGroupParam.m_strDestroyType = bb.InnerText;
                                                    //             }
                                                    //             break;
                                                    //         case "GridLineCol":
                                                    //             {
                                                    //                 XmlNodeList bbbList = bb.ChildNodes;
                                                    //                 int nChessBoardIndex = 0;
                                                    //                 int nLine = 0;
                                                    //                 int nCol = 0;
                                                    //                 foreach (XmlNode bbb in bbbList)
                                                    //                 {
                                                    //                     switch (bbb.Name)
                                                    //                     {
                                                    //                         case "Chess":
                                                    //                             {
                                                    //                                 nChessBoardIndex = int.Parse(bbb.InnerText);
                                                    //                             }
                                                    //                             break;
                                                    //                         case "Line":
                                                    //                             {
                                                    //                                 nLine = int.Parse(bbb.InnerText);
                                                    //                             }
                                                    //                             break;
                                                    //                         case "Col":
                                                    //                             {
                                                    //                                 nCol = int.Parse(bbb.InnerText);
                                                    //                             }
                                                    //                             break;
                                                    //                     }
                                                    //                 }
                                                    //                 tGroupParam.addLineCol(nChessBoardIndex, nLine, nCol);
                                                    //             }
                                                    //             break;
                                                    //     }
                                                    // }
                                                    // m_arrGroupParam.Add(tGroupParam);
                                                }
                                                break;
                                        }

                                    }
                                }
                                break;
                            case "board":
                                {
                                    BoardParam objBoardParam = new BoardParam();
                                    XmlNodeList bList = gc.ChildNodes;
                                    foreach (XmlNode b in bList)
                                    {
                                        switch (b.Name)
                                        {
                                            case "boardId":
                                                {
                                                    objBoardParam.nId = int.Parse(b.InnerText);
                                                    break;
                                                }
                                            case "boardSize":
                                                {
                                                    string strSize = b.InnerText;
                                                    string[] strPos = strSize.Split(',');
                                                    objBoardParam.nRow = int.Parse(strPos[0]);
                                                    objBoardParam.nCol = int.Parse(strPos[1]);
                                                    break;
                                                }
                                            case "IsConnectLastChessBoard":
                                                {
                                                    objBoardParam.m_bIsConnectLastChessBoard = bool.Parse(b.InnerText);
                                                }
                                                break;
                                            case "ConnectDirection":
                                                {
                                                    objBoardParam.m_eConnectDirection = (Direction) int.Parse(b.InnerText);
                                                }
                                                break;
                                            case "block":
                                                {
                                                    CellGridParam objCellGridParam = new CellGridParam();
                                                    XmlNodeList blkList = b.ChildNodes;
                                                    foreach (XmlNode blk in blkList)
                                                    {
                                                        switch (blk.Name)
                                                        {
                                                            case "boardPos":
                                                                {
                                                                    string strBoardSize = blk.InnerText;
                                                                    string[] strBoardPos = strBoardSize.Split(',');
                                                                    objCellGridParam.nPosX = int.Parse(strBoardPos[0]);
                                                                    objCellGridParam.nPosY = int.Parse(strBoardPos[1]);
                                                                    break;
                                                                }
                                                            case "boardDrop":
                                                                {
                                                                    objCellGridParam.strDropId = blk.InnerText;
                                                                    break;
                                                                }
                                                            case "dropdirection":
                                                                {
                                                                    objCellGridParam.intDirection = int.Parse(blk.InnerText);
                                                                    break;
                                                                }
                                                            case "elementId":
                                                                {
                                                                    objCellGridParam.m_listElementId.Add(blk.InnerText);
                                                                    break;
                                                                }
                                                        }
                                                    }
                                                    objBoardParam.m_listCellGrid.Add(objCellGridParam);
                                                    break;
                                                }
                                        }
                                    }
                                    m_listBoard.Add(objBoardParam);
                                    break;
                                }
                        }
                    }
                }
            }
        }

    }

}