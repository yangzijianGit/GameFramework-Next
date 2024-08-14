using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ENate
{
    public struct GridCoord
    {
        int m_nChessBoardIndex;
        int m_nLine;
        int m_nCol;
        int m_nCoord;

        static GridCoord m_tNull = new GridCoord(-1, -1, -1);
        static GridCoord m_tZero = new GridCoord(0, 0, 0);

        public static GridCoord NULL
        {
            get
            {
                return m_tNull;
            }
        }
        public static GridCoord Zero
        {
            get
            {
                return m_tZero;
            }
        }

        public override bool Equals(object obj)
        {
            GridCoord tCompare = (GridCoord) obj;
            return tCompare.Coord == this.Coord;
        }

        public bool isNull()
        {
            return Equals(NULL);
        }

        public int ChessBoardIndex
        {
            get { return m_nChessBoardIndex; }
        }
        public int Line
        {
            get { return m_nLine; }
        }
        public int Col
        {
            get { return m_nCol; }
        }
        public int Coord
        {
            get { return m_nCoord; }
        }

        public GridCoord(int nChessBoardIndex, int nLine, int nCol)
        {
            m_nChessBoardIndex = nChessBoardIndex;
            m_nLine = nLine;
            m_nCol = nCol;
            m_nCoord = posToCoord(m_nChessBoardIndex, m_nLine, m_nCol);
        }
        public GridCoord(GridCoord tGridCoord)
        {
            m_nChessBoardIndex = tGridCoord.m_nChessBoardIndex;
            m_nLine = tGridCoord.m_nLine;
            m_nCol = tGridCoord.m_nCol;
            m_nCoord = tGridCoord.m_nCoord;
        }

        public void coord(int nChessBoardIndex, int nLine, int nCol)
        {
            m_nChessBoardIndex = nChessBoardIndex;
            m_nLine = nLine;
            m_nCol = nCol;
            m_nCoord = posToCoord(this);
        }

        public static int posToCoord(int nIndex, int nLine, int nCol)
        {
            if (nIndex == -1 && nLine == -1 && nCol == -1)
            {
                return -1;
            }
            return nIndex * 10000 + nLine * 100 + nCol;
        }

        public static int posToCoord(GridCoord tGridCoord)
        {
            return posToCoord(tGridCoord.m_nChessBoardIndex, tGridCoord.m_nLine, tGridCoord.m_nCol);
        }

        public static GridCoord CoordToPos(int nCoord)
        {
            int nCol = nCoord % 100;
            nCoord /= 100;
            int nLine = nCoord % 100;
            int nChessBoardIndex = nCoord / 100;
            GridCoord tGridCoord = new GridCoord(nChessBoardIndex, nLine, nCol);
            return tGridCoord;
        }
    }
    public static class Util
    {
        // The Lines and Columns correspond to y and x.
        public static GridCoord getDirectionLineCol(int nChessBoardIndex, int nLine, int nColumn, ref Direction eDirection)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getDirectionLineCol");
            GridCoord tReVec2i = GridCoord.NULL;
            switch (eDirection)
            {
                case Direction.UpLeft:
                    {
                        tReVec2i = new GridCoord(nChessBoardIndex, nLine - 1, nColumn - 1);
                    }
                    break;
                case Direction.Up:
                    {
                        tReVec2i = new GridCoord(nChessBoardIndex, nLine - 1, nColumn);
                    }
                    break;
                case Direction.UpRight:
                    {
                        tReVec2i = new GridCoord(nChessBoardIndex, nLine - 1, nColumn + 1);
                    }
                    break;
                case Direction.Left:
                    {
                        tReVec2i = new GridCoord(nChessBoardIndex, nLine, nColumn - 1);
                    }
                    break;
                case Direction.Mine:
                    {
                        tReVec2i = new GridCoord(nChessBoardIndex, nLine, nColumn);
                    }
                    break;
                case Direction.Right:
                    {
                        tReVec2i = new GridCoord(nChessBoardIndex, nLine, nColumn + 1);
                    }
                    break;
                case Direction.DownLeft:
                    {
                        tReVec2i = new GridCoord(nChessBoardIndex, nLine + 1, nColumn - 1);
                    }
                    break;
                case Direction.Down:
                    {
                        tReVec2i = new GridCoord(nChessBoardIndex, nLine + 1, nColumn);
                    }
                    break;
                case Direction.DownRight:
                    {
                        tReVec2i = new GridCoord(nChessBoardIndex, nLine + 1, nColumn + 1);
                    }
                    break;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return tReVec2i;
        }
        public static GridCoord getDirectionLineCol(int nChessBoardIndex, int nLine, int nColumn, Direction eDirection)
        {
            return getDirectionLineCol(nChessBoardIndex, nLine, nColumn, ref eDirection);
        }
        public static GridCoord getDirectionLineCol(ref GridCoord tVec, ref Direction eDirection)
        {
            return getDirectionLineCol(tVec.ChessBoardIndex, tVec.Line, tVec.Col, ref eDirection);
        }
        public static GridCoord getDirectionLineCol(ref GridCoord tVec, Direction eDirection)
        {
            return getDirectionLineCol(tVec.ChessBoardIndex, tVec.Line, tVec.Col, ref eDirection);
        }
        public static Direction getDirectionOpposite(Direction eDirection)
        {
            switch (eDirection)
            {
                case Direction.UpLeft:
                    return Direction.DownRight;
                case Direction.Up:
                    return Direction.Down;
                case Direction.UpRight:
                    return Direction.DownLeft;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.DownLeft:
                    return Direction.UpRight;
                case Direction.Down:
                    return Direction.Up;
                case Direction.DownRight:
                    return Direction.UpLeft;
            }
            return Direction.NULL;
        }

        public static int getDirectionFixType(Direction tDirection, bool bIsSelf)
        {
            switch (tDirection)
            {
                case Direction.Up:
                    {
                        return bIsSelf ? 2 : 3;
                    }
                case Direction.Down:
                    {
                        return bIsSelf ? 3 : 2;
                    }

                case Direction.Left:
                    {
                        return bIsSelf ? 4 : 5;
                    }
                case Direction.Right:
                    {
                        return bIsSelf ? 5 : 4;
                    }
            }
            return 0;
        }

        public static Direction getDirectionBranch(Direction eDirection, bool bIsLeftBranch)
        {
            int nDirection = (int) eDirection;
            if (bIsLeftBranch)
            {
                nDirection++;
            }
            else
            {
                nDirection--;
            }
            if (nDirection < 0)
            {
                nDirection = 0;
            }
            if (nDirection > (int) Direction.Left)
            {
                nDirection = (int) Direction.UpLeft;
            }
            return (Direction) nDirection;
        }
        ///
        public static Direction getDirectionWithLineCol(int nLine, int nColumn, int nLine2, int nColumn2)
        {
            int offectLine = nLine2 - nLine;
            int offectColumn = nColumn2 - nColumn;
            if (offectLine > 0)
            {
                if (offectColumn > 0)
                {
                    return Direction.DownRight;
                }
                else if (offectColumn < 0)
                {
                    return Direction.DownLeft;
                }
                else
                {
                    return Direction.Down;
                }
            }
            else if (offectLine < 0)
            {
                if (offectColumn > 0)
                {
                    return Direction.UpRight;
                }
                else if (offectColumn < 0)
                {
                    return Direction.UpLeft;
                }
                else
                {
                    return Direction.Up;
                }
            }
            else
            {
                if (offectColumn > 0)
                {
                    return Direction.Right;
                }
                else if (offectColumn < 0)
                {
                    return Direction.Left;
                }
                else
                {
                    return Direction.Mine;
                }
            }
        }

        public static void getDirectionGrid(ChessBoard tChessBoard, GridCoord tGridCoord, Direction eDirection, ref List<Grid> arrGrid, bool bIsContainsSelfGrid = true)
        {
            int nChessBoardLineMax = tChessBoard.m_nHeight;
            int nChessBoardColumnMax = tChessBoard.m_nWidth;
            switch (eDirection)
            {
                case Direction.Up:
                    {
                        for (int nLine = tGridCoord.Line; nLine >= 0; --nLine)
                        {
                            Grid tGrid = tChessBoard.getGrid(nLine, tGridCoord.Col);
                            if (tGrid != null)
                                arrGrid.Add(tGrid);
                        }
                        break;
                    }
                case Direction.Down:
                    {
                        for (int nLine = tGridCoord.Line; nLine < nChessBoardLineMax; ++nLine)
                        {
                            Grid tGrid = tChessBoard.getGrid(nLine, tGridCoord.Col);
                            if (tGrid != null)
                                arrGrid.Add(tGrid);
                        }
                        break;
                    }
                case Direction.Left:
                    {
                        for (int nColumn = tGridCoord.Col; nColumn >= 0; --nColumn)
                        {
                            Grid tGrid = tChessBoard.getGrid(tGridCoord.Line, nColumn);
                            if (tGrid != null)
                                arrGrid.Add(tGrid);
                        }
                        break;
                    }
                case Direction.Right:
                    {
                        for (int nColumn = tGridCoord.Col; nColumn < nChessBoardColumnMax; ++nColumn)
                        {
                            Grid tGrid = tChessBoard.getGrid(tGridCoord.Line, nColumn);
                            if (tGrid != null)
                                arrGrid.Add(tGrid);
                        }
                        break;
                    }
                default:
                    {
                        Debug.LogError("ERROR: the current direction is not supported. ");
                        break;
                    }
            }
            if (bIsContainsSelfGrid == false)
            {
                var tMineGrid = tChessBoard.getGrid(tGridCoord);
                if (tMineGrid != null)
                    arrGrid.Remove(tMineGrid);
            }
        }

        static public ElementDestroy checkPassDestroy(this Grid tGrid)
        {
            ElementDestroy tElementDestroy = new ElementDestroy();
            if (tGrid == null)
            {
                return tElementDestroy;
            }
            foreach (var tElement in tGrid.m_sortedElement)
            {
                PassCreate tPassCreate = tElement.Value.getElementAttribute(ElementAttribute.Attribute.passCreateDestroy) as PassCreate;
                if (tPassCreate == null)
                {
                    continue;
                }
                ConditionConfig.MapArg mp = new ConditionConfig.MapArg();
                mp.ChessBoard = tGrid.m_tChessBoard;
                mp.Stage = tGrid.m_tChessBoard.m_tStage;
                mp.Grid = tGrid;
                if (ConditionConfig.checkCondition(tPassCreate.m_strConditionId, mp) == true)
                {
                    tElementDestroy.addDestroyType(tPassCreate.m_tElementDestroy);
                }
            }
            return tElementDestroy;
        }

        static public ElementDestroy getDirectionConnectDestroyType(Direction eDirection)
        {
            ElementDestroy tElementDestroy = new ElementDestroy();
            switch (eDirection)
            {
                case Direction.Up:
                    {
                        tElementDestroy.addDestroyType(12);
                    }
                    break;
                case Direction.Down:
                    {
                        tElementDestroy.addDestroyType(13);
                    }
                    break;
                case Direction.Left:
                    {
                        tElementDestroy.addDestroyType(10);
                    }
                    break;
                case Direction.Right:
                    {
                        tElementDestroy.addDestroyType(11);
                    }
                    break;
            }
            return tElementDestroy;
        }

        static public Vector2? parseVector2(this string str)
        {
            if (string.IsNullOrEmpty(str) == true)
            {
                return null;
            }
            try
            {
                var arrSplit = str.Split(',');
                return new Vector2(float.Parse(arrSplit[0]), float.Parse(arrSplit[1]));
            }
            catch (Exception ex)
            {
                Debug.LogError( ex.ToString());
            }
            return null;
        }
        static public Vector2Int? parseVector2Int(this string str)
        {
            if (string.IsNullOrEmpty(str) == true)
            {
                return null;
            }
            try
            {
                var arrSplit = str.Split(',');
                return new Vector2Int(int.Parse(arrSplit[0]), int.Parse(arrSplit[1]));
            }
            catch (Exception ex)
            {
                Debug.LogError( ex.ToString());
            }
            return null;
        }
        static public Vector3? parseVector3(this string str)
        {
            if (string.IsNullOrEmpty(str) == true)
            {
                return null;
            }
            try
            {
                var arrSplit = str.Split(',');
                return new Vector3(float.Parse(arrSplit[0]), float.Parse(arrSplit[1]), float.Parse(arrSplit[2]));
            }
            catch (Exception ex)
            {
                Debug.LogError( ex.ToString());
            }
            return null;
        }
        static public Vector4? parseVector4(this string str)
        {
            if (string.IsNullOrEmpty(str) == true)
            {
                return null;
            }
            try
            {
                var arrSplit = str.Split(',');
                return new Vector4(float.Parse(arrSplit[0]), float.Parse(arrSplit[1]), float.Parse(arrSplit[2]), float.Parse(arrSplit[3]));
            }
            catch (Exception ex)
            {
                Debug.LogError( ex.ToString());
            }
            return null;
        }
        static public int? parseInt(this string str)
        {
            if (string.IsNullOrEmpty(str) == true)
            {
                return null;
            }
            try
            {
                var arrSplit = str.Split(',');
                return int.Parse(str);
            }
            catch (Exception ex)
            {
                Debug.LogError( ex.ToString());
            }
            return null;
        }
        static public float? parsefloat(this string str)
        {
            if (string.IsNullOrEmpty(str) == true)
            {
                return null;
            }
            try
            {
                var arrSplit = str.Split(',');
                return float.Parse(str);
            }
            catch (Exception ex)
            {
                Debug.LogError( ex.ToString());
            }
            return null;
        }
        static public bool? parsebool(this string str)
        {
            if (string.IsNullOrEmpty(str) == true)
            {
                return null;
            }
            try
            {
                var arrSplit = str.Split(',');
                return bool.Parse(str);
            }
            catch (Exception ex)
            {
                Debug.LogError( ex.ToString());
            }
            return null;
        }

        static public Vector4 mulEverySub(this Vector4 v3f, Vector4 v3ft)
        {
            return new Vector4(v3f.x * v3ft.x, v3f.y * v3ft.y, v3f.z * v3ft.z, v3f.w * v3ft.w);
        }
        static public Vector3 mulEverySub(this Vector3 v3f, Vector3 v3ft)
        {
            return new Vector3(v3f.x * v3ft.x, v3f.y * v3ft.y, v3f.z * v3ft.z);
        }
        static public Vector2 mulEverySub(this Vector2 v3f, Vector2 v3ft)
        {
            return new Vector2(v3f.x * v3ft.x, v3f.y * v3ft.y);
        }

        static public int getExcuteOperatorMarkId(int nExcuteCounterId, int nOperatorId)
        {
            return nExcuteCounterId * 1000000 + (nOperatorId % 1000000);
        }

        public static void playENateAni(string strAniId, GameObject obj = null, Vector3? v3fRoot = null, Vector3? v3fTrigger = null, Action fcCallback = null, bool isAddLockQueue = true)
        {
            if (string.IsNullOrEmpty(strAniId) == true)
            {
                return;
            }
            ENateAnimation.ENateEventArg tEventArg = new ENateAnimation.ENateEventArg();
            tEventArg.strAnimationId = strAniId;
            tEventArg.tENateAniArg = new ENateAniArg();
            tEventArg.tENateAniArg.tObj = obj;
            if (v3fRoot != null) tEventArg.tENateAniArg.tRootPos = v3fRoot.Value;
            if (v3fTrigger != null) tEventArg.tENateAniArg.tTriggerPos = v3fTrigger.Value;
            tEventArg.pCallBack = fcCallback;
            tEventArg.isAddLockQueue = isAddLockQueue;
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_Ani_Play, tEventArg);
        }

        public static string getExchangeAniId(bool isFront, Direction eDirection)
        {
            string strExchangeAniId = "exchange";
            if (isFront == true)
            {
                strExchangeAniId += "F_";
            }
            else
            {
                strExchangeAniId += "B_";
            }
            switch (eDirection)
            {
                case Direction.Up:
                    {
                        strExchangeAniId += "U";
                    }
                    break;
                case Direction.Down:
                    {
                        strExchangeAniId += "D";
                    }
                    break;
                case Direction.Left:
                    {
                        strExchangeAniId += "L";
                    }
                    break;
                case Direction.Right:
                    {
                        strExchangeAniId += "R";
                    }
                    break;
            }
            return strExchangeAniId;
        }
    }
}