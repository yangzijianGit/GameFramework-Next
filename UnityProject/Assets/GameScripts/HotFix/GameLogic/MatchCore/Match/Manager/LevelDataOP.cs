using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

using System.IO;
using System.Xml;
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
    public int intDirection = -1;
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

    //格子列表
    public List<CellGridParam> m_listCellGrid = new List<CellGridParam>();

    public bool m_bIsConnectLastChessBoard = false;
    /*
    public enum Direction
    {
        Mine = 0,
        UpLeft = 1,
        Up = 2,
        UpRight = 3,
        Right = 4,
        DownRight = 5,
        Down = 6,
        DownLeft = 7,
        Left = 8,
        NULL
    }
    */
    public int m_nConnectDirection = 6;

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

//关卡数据类
public sealed class LevelDataOP
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

    public List<GroupParam> m_arrGroupParam = new List<GroupParam>();

    /** 辅助变量(不写入文件) **/
    //参数不完整类型、布局不完整类型(-1不完整错误，1不完整，0完整)
    public int m_nParamIncompleteType = 0;
    public int m_nLayoutIncompleteType = 0;
    //是否是新增关卡
    public bool m_bIsAddLevel = false;

    /** 构造 **/
    public LevelDataOP() { }

    /** 清理 **/
    public void Clear()
    {
        this.ClearParam();
        this.ClearBoard();
    }

    public void ClearParam()
    {
        this.m_nParamIncompleteType = 0;
        this.m_nCreateWay = 0;
        this.m_strCreateTime = string.Empty;
        this.m_strFileName = string.Empty;
        this.m_strLevelName = string.Empty;
        this.m_nStep = 0;
        this.m_nTime = 0;
        this.m_nNPC = 0;
        foreach (var element in this.m_listElement)
        {
            element.Clear();
        }
        this.m_listElement.Clear();
    }

    public void ClearBoard()
    {
        this.m_nLayoutIncompleteType = 0;
        foreach (var board in this.m_listBoard)
        {
            board.Clear();
        }
        this.m_listBoard.Clear();
    }
}

//关卡数据管理(无实例)
public sealed class LevelDataManagerOP
{
    //单个关卡数据
    public static LevelDataOP objLevelData = new LevelDataOP();

    //关卡数据列表，关卡数据映射列表<关卡文件名，关卡数据>(快速查询)
    public static List<LevelDataOP> listLevelData = new List<LevelDataOP>();
    public static Dictionary<string, LevelDataOP> mapLevelData = new Dictionary<string, LevelDataOP>();

    /** 构造函数 **/
    private LevelDataManagerOP() { }

    //获取路径关卡路径
    private static string getLevelPath(string levelFileName)
    {
        string strSub = "artist/201911_kumamon/Assets";
        int index = Application.dataPath.IndexOf(strSub);
        string strPrefix = Application.dataPath.Substring(0, index);
        // string allPath = ConfigOP.m_strLevelOutputPath + "/" + levelFileName + ".xml";

        // return allPath;
        return null;
    }

    //同步关卡数据
    public static LevelDataOP SynLevelData()
    {
        //判断关卡是否存在
        // string strTime = Function.GetTimeString();
        LevelDataOP levelData = LevelDataManagerOP.objLevelData;

        levelData = LevelDataManagerOP.objLevelData;
        levelData.m_bIsAddLevel = true;
        levelData.m_nCreateWay = 1;
        // levelData.m_strCreateTime = strTime;
        // levelData.m_strLastTime = strTime;

        LevelDataManagerOP.listLevelData.Add(levelData);
        LevelDataManagerOP.mapLevelData[levelData.m_strFileName] = levelData;
        //置空关卡
        LevelDataManagerOP.objLevelData = new LevelDataOP();

        return levelData;
    }

    //保存数据
    public static void Save(LevelDataOP data)
    {
        string fileAllPath = LevelDataManagerOP.getLevelPath(data.m_strFileName);

        string xml = string.Empty;
        xml += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        xml += "<root>";
        xml += "<client>1</client>";
        xml += "<game gameId=\"-99\">";

        //**写入头部
        xml += "<createWay>" + data.m_nCreateWay + "</createWay>";
        xml += "<createTime>" + data.m_strCreateTime + "</createTime>";
        xml += "<lastTime>" + data.m_strLastTime + "</lastTime>";

        //**写入参数
        //系统参数
        xml += "<sysParam>";
        xml += "<levelId>" + data.m_strLevelName + "</levelId>";
        xml += "</sysParam>";

        //通关参数
        xml += "<passParam>";
        xml += "<limitStep>" + data.m_nStep + "</limitStep>";
        xml += "<limitNPC>" + data.m_nNPC + "</limitNPC>";
        xml += "<limitElement>";
        foreach (var element in data.m_listElement)
        {
            xml += "<element>";
            xml += "<name>" + element.strName + "</name>";
            xml += "<type>" + element.nType + "</type>";
            if (element.nType == 1)
            {
                xml += "<count>" + element.nCount + "</count>";
            }
            xml += "<boardId>" + element.nBoard + "</boardId>";
            xml += "</element>";
        }
        xml += "</limitElement>";
        xml += "</passParam>";

        // 组信息
        xml += "<groupNode>";
        foreach (var itGroup in data.m_arrGroupParam)
        {
            xml += "<groupInfo>";
            xml += "<GroupId>" + itGroup.m_strGroupId + "</GroupId>";
            xml += "<ElementId>" + itGroup.m_strElementId + "</ElementId>";
            xml += "<DestroyType>" + itGroup.m_strDestroyType + "</DestroyType>";
            foreach (var tCell in itGroup.m_arrGridLineCol)
            {
                xml += "<GridLineCol>";

                xml += "<Chess>" + tCell.m_nChessBoardIndex + "</Chess>";
                xml += "<Line>" + tCell.m_nLine + "</Line>";
                xml += "<Col>" + tCell.m_nCol + "</Col>";
                xml += "</GridLineCol>";
            }
            xml += "</groupInfo>";
        }
        xml += "</groupNode>";

        //**写入布局
        foreach (var board in data.m_listBoard)
        {
            xml += "<board>";
            xml += "<boardId>" + board.nId + "</boardId>";
            xml += "<boardSize>" + board.nRow + "," + board.nCol + "</boardSize>";
            xml += "<IsConnectLastChessBoard>" + board.m_bIsConnectLastChessBoard + "</IsConnectLastChessBoard>";
            xml += "<ConnectDirection>" + board.m_nConnectDirection + "</ConnectDirection>";
            foreach (var cell in board.m_listCellGrid)
            {
                xml += "<block>";
                xml += "<boardPos>" + cell.nPosX + "," + cell.nPosY + "</boardPos>";
                if (cell.strDropId != string.Empty)
                {
                    xml += "<boardDrop>" + cell.strDropId + "</boardDrop>";
                }
                xml += "<dropdirection>" + cell.intDirection + "</dropdirection>";
                foreach (string id in cell.m_listElementId)
                {
                    xml += "<elementId>" + id + "</elementId>";
                }
                xml += "</block>";
            }
            xml += "</board>";
        }

        //**写入尾部
        xml += "</game>";
        xml += "</root>";

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        doc.Save(fileAllPath);

        Debug.Log(string.Format("LevelDataManagerOP - Save - Save Level Finished! \"{0}\"", fileAllPath));

        //判断是否是新增
        if (data.m_bIsAddLevel)
        {
            data.m_bIsAddLevel = false;
            // EventOP.NoticeEvent((int) EventTypeOP.ET_ADD_LEVEL, new ETLevelParam(data));
        }
    }

    //批量保存数据
    public static void Save()
    {
        foreach (var data in LevelDataManagerOP.listLevelData)
        {
            LevelDataManagerOP.Save(data);
        }
    }

    //读取数据
    public static void Load(string fileName)
    {
        LevelDataOP data = new LevelDataOP();
        data.m_strFileName = fileName;

        string fileAllPath = LevelDataManagerOP.getLevelPath(fileName);

        FileStream mStream = File.Open(fileAllPath, FileMode.Open, FileAccess.Read);
        XmlDocument xml = new XmlDocument();
        xml.Load(mStream);
        mStream.Flush();
        mStream.Close();

        //XmlTextReader reader = new XmlTextReader(mStream); 
        //if (null == reader) {
        //    Debug.LogError(string.Format("LevelDataOP - Load - Read XML Failed! \"{0}\"", fileAllPath));
        //    return;
        //}
        //while (reader.Read()) {
        //XmlDocument xmlDocument = new XmlDocument();
        //XmlNode node = xmlDocument.ReadNode(reader);

        XmlNodeList xmlChild = xml.ChildNodes;
        XmlNodeList rootChild = null;
        foreach (XmlNode rc in xmlChild)
        {
            if (rc.Name == "root")
            {
                rootChild = rc.ChildNodes;
            }
        }
        if (rootChild == null) return;
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
                                data.m_nCreateWay = int.Parse(gc.InnerText);
                                break;
                            }
                        case "createTime":
                            {
                                data.m_strCreateTime = gc.InnerText;
                                break;
                            }
                        case "lastTime":
                            {
                                data.m_strLastTime = gc.InnerText;
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
                                                data.m_strLevelName = sp.InnerText;
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
                                                data.m_nStep = int.Parse(pp.InnerText);
                                                break;
                                            }
                                        case "limitNPC":
                                            {
                                                data.m_nNPC = int.Parse(pp.InnerText);
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
                                                                data.m_listElement.Add(objElementParam);
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
                                                GroupParam tGroupParam = new GroupParam();
                                                XmlNodeList bbList = b.ChildNodes;
                                                foreach (XmlNode bb in bbList)
                                                {
                                                    switch (bb.Name)
                                                    {
                                                        case "GroupId":
                                                            {
                                                                tGroupParam.m_strGroupId = bb.InnerText;
                                                            }
                                                            break;
                                                        case "ElementId":
                                                            {
                                                                tGroupParam.m_strElementId = bb.InnerText;
                                                            }
                                                            break;
                                                        case "DestroyType":
                                                            {
                                                                tGroupParam.m_strDestroyType = bb.InnerText;
                                                            }
                                                            break;
                                                        case "GridLineCol":
                                                            {
                                                                XmlNodeList bbbList = bb.ChildNodes;
                                                                int nChessBoardIndex = 0;
                                                                int nLine = 0;
                                                                int nCol = 0;
                                                                foreach (XmlNode bbb in bbbList)
                                                                {
                                                                    switch (bbb.Name)
                                                                    {
                                                                        case "Chess":
                                                                            {
                                                                                nChessBoardIndex = int.Parse(bbb.InnerText);
                                                                            }
                                                                            break;
                                                                        case "Line":
                                                                            {
                                                                                nLine = int.Parse(bbb.InnerText);
                                                                            }
                                                                            break;
                                                                        case "Col":
                                                                            {
                                                                                nCol = int.Parse(bbb.InnerText);
                                                                            }
                                                                            break;
                                                                    }
                                                                }
                                                                tGroupParam.addLineCol(nChessBoardIndex, nLine, nCol);
                                                            }
                                                            break;
                                                    }
                                                }
                                                data.m_arrGroupParam.Add(tGroupParam);
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
                                                objBoardParam.m_nConnectDirection = int.Parse(b.InnerText);
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
                                data.m_listBoard.Add(objBoardParam);
                                break;
                            }
                    }
                }
            }
        }
        //}

        //reader.Close();

        LevelDataManagerOP.listLevelData.Add(data);
        LevelDataManagerOP.mapLevelData[data.m_strFileName] = data;
    }

    //批量读取数据
    public static void Load()
    {
        string strSub = "artist/201911_kumamon/Assets";
        int index = Application.dataPath.IndexOf(strSub);
        string strPrefix = Application.dataPath.Substring(0, index);
        string filePath = strPrefix + GlobalDefine.PATH_OUTPUT_LEVEL_ROOT;

        DirectoryInfo root = new DirectoryInfo(filePath);
        foreach (FileInfo fi in root.GetFiles())
        {
            if (fi.Name.IndexOf(".xml") != -1)
            {
                string fileName = Path.GetFileNameWithoutExtension(fi.Name);
                LevelDataManagerOP.Load(fileName);
            }
        }
    }

    //删除关卡数据
    public static void Delete(string fileLevelName)
    {
        //判断是否有关卡
        if (!LevelDataManagerOP.mapLevelData.ContainsKey(fileLevelName))
        {
            Debug.LogError(string.Format("LevelDataManagerOP - Delete - Not Found Level! \"{0}\"", fileLevelName));
            return;
        }

        //通知删除关卡
        EventOP.NoticeEvent((int) EventTypeOP.ET_DEL_LEVEL, new ETLevelParam(LevelDataManagerOP.mapLevelData[fileLevelName]));

        //从列表删除
        LevelDataOP levelData = null;
        foreach (var level in LevelDataManagerOP.listLevelData)
        {
            if (level.m_strFileName == fileLevelName)
            {
                levelData = level;
                break;
            }
        }

        LevelDataManagerOP.listLevelData.Remove(levelData);
        LevelDataManagerOP.mapLevelData.Remove(fileLevelName);

        //删除文件
        string fileAllPath = LevelDataManagerOP.getLevelPath(fileLevelName);
        if (!File.Exists(fileAllPath))
        {
            Debug.LogError(string.Format("LevelDataManagerOP - Delete - Not Found Level File! \"{0}\"", fileAllPath));
            return;
        }
        File.Delete(fileAllPath);
    }
}

#endif

public class GroupParam
{
    public class GridLineCol
    {
        public int m_nChessBoardIndex;
        public int m_nLine;
        public int m_nCol;

        public GridLineCol(int nChessBoardIndex, int nLine, int nCol)
        {
            m_nChessBoardIndex = nChessBoardIndex;
            m_nLine = nLine;
            m_nCol = nCol;
        }
    }

    public string m_strDestroyType;
    public string m_strElementId;
    public string m_strGroupId;

    public List<GridLineCol> m_arrGridLineCol = new List<GridLineCol>();

    public void addLineCol(int nChessBoardIndex, int nLine, int nCol)
    {
        m_arrGridLineCol.Add(new GridLineCol(nChessBoardIndex, nLine, nCol));
    }

}