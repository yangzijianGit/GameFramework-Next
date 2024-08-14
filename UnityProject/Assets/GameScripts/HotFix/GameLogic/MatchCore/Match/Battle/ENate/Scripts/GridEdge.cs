using UnityEditor;
using UnityEngine;

namespace ENate
{

    public class GridEdge : MonoBehaviour
    {
        public float battleEdgeInSideWidth;
        public float battleEdgeOutSideWidth;
        public float battleGridWidth;
        public float battleEdgeWidth;
        public GameObject grid_floorOutSide;
        public GameObject grid_floorInside;
        public GameObject grid_line;
        public GameObject parent;
        Stage m_tStage;
        public ChessBoard m_tChessBoard;

        public jc.EventManager.EventObj m_tEventObj;
        private void OnEnable()
        {
            m_tEventObj = new jc.EventManager.EventObj();
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Init, event_init);
        }

        private void OnDisable()
        {
            m_tEventObj.clear();
            m_tEventObj = null;
        }

        void event_init(object o)
        {
            m_tStage = o as Stage;
            show();
        }
        public class GridEdgeOffect
        {
            GridEdge m_tGridEdge;
            public int m_nAngle;
            public float m_fOffectGridX;
            public float m_fOffectGridY;
            public GameObject m_tEdgeId;
            public GridEdgeOffect(GridEdge tGridEdge)
            {
                m_tGridEdge = tGridEdge;
                m_nAngle = 0;
                m_fOffectGridX = 0.0f;
                m_fOffectGridY = 0.0f;
                m_tEdgeId = null;
            }
            
            public void copyAndSetInSide(float fPosX, float fPosY)
            {
                GameObject tEdgeObj = GameObject.Instantiate(m_tEdgeId);
                if (m_tGridEdge.parent == null)
                {
                    tEdgeObj.attachObj(m_tGridEdge.gameObject);
                }
                else
                {
                    tEdgeObj.attachObj(m_tGridEdge.parent);
                }
                tEdgeObj.transform.localEulerAngles = new Vector3(0, 0, m_nAngle);
                tEdgeObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(fPosX + m_fOffectGridX, fPosY + m_fOffectGridY);
            }
        }

        void battle_set3GridAnglePosition(int nGridBit, float fPosX, float fPosY)
        {
            /** inside grid edge **/
            GridEdgeOffect tGrid3Angle = new GridEdgeOffect(this);
            while (true)
            {
                float fWidth = battleGridWidth;
                float fInSideWidth = battleEdgeInSideWidth;

                tGrid3Angle.m_tEdgeId = grid_floorInside;
                if (nGridBit - 8 < 0) // 左边
                {
                    tGrid3Angle.m_tEdgeId = grid_floorInside;
                    tGrid3Angle.m_fOffectGridX = -fWidth / 2.0f - fInSideWidth / 2;
                    tGrid3Angle.m_fOffectGridY = fWidth / 2 - fInSideWidth / 2;
                    break;
                }
                nGridBit = nGridBit - 8;
                if (nGridBit - 4 < 0) // 左上
                {
                    tGrid3Angle.m_nAngle = 270; // 左上
                    tGrid3Angle.m_fOffectGridX = -fWidth / 2.0f - fInSideWidth / 2;
                    tGrid3Angle.m_fOffectGridY = fWidth / 2 + fInSideWidth / 2;
                    break;
                }
                nGridBit = nGridBit - 4;
                if (nGridBit - 2 < 0) // 右上
                {
                    tGrid3Angle.m_nAngle = 180; // 右上
                    tGrid3Angle.m_fOffectGridX = -fWidth / 2.0f + fInSideWidth / 2;
                    tGrid3Angle.m_fOffectGridY = fWidth / 2 + fInSideWidth / 2;
                    break;
                }
                tGrid3Angle.m_nAngle = 90; // 右下
                tGrid3Angle.m_fOffectGridX = -fWidth / 2.0f + fInSideWidth / 2;
                tGrid3Angle.m_fOffectGridY = fWidth / 2 - fInSideWidth / 2;
                break;
            }
            tGrid3Angle.copyAndSetInSide(fPosX, fPosY);
        }

        void battle_set2GridAnglePosition(int nGridBit, float fPosX, float fPosY)
        {
            /** line or opposite **/
            GridEdgeOffect tGridAngle1 = null;
            GridEdgeOffect tGridAngle2 = null;
            while (true)
            {
                float fWidth = battleGridWidth;
                float fEdgeWidth = battleEdgeWidth;
                float fInSideWidth = battleEdgeInSideWidth;

                if (nGridBit - 8 < 0) // 左边区域是空的
                {
                    if (nGridBit - 4 < 0) // 左上竖条
                    {
                        tGridAngle1 = new GridEdgeOffect(this);
                        tGridAngle1.m_tEdgeId = grid_line;
                        tGridAngle1.m_nAngle = 90;
                        tGridAngle1.m_fOffectGridX = -fWidth / 2.0f - fEdgeWidth / 2.0f;
                        tGridAngle1.m_fOffectGridY = fWidth / 2;
                        break;
                    }
                    nGridBit = nGridBit - 4;
                    if (nGridBit - 2 < 0) // 本身 到 左上对角有方块
                    {
                        tGridAngle1 = new GridEdgeOffect(this);
                        tGridAngle1.m_tEdgeId = grid_floorInside;
                        tGridAngle1.m_nAngle = 180; // 右上
                        tGridAngle1.m_fOffectGridX = -fWidth / 2.0f + fInSideWidth / 2;
                        tGridAngle1.m_fOffectGridY = fWidth / 2 + fInSideWidth / 2;
                        tGridAngle2 = new GridEdgeOffect(this);
                        tGridAngle2.m_tEdgeId = grid_floorInside; // 左下
                        tGridAngle2.m_fOffectGridX = -fWidth / 2.0f - fInSideWidth / 2;
                        tGridAngle2.m_fOffectGridY = fWidth / 2 - fInSideWidth / 2;
                        break;
                    }
                    tGridAngle1 = new GridEdgeOffect(this); // 上面横向两个
                    tGridAngle1.m_tEdgeId = grid_line;
                    tGridAngle1.m_nAngle = 180;
                    tGridAngle1.m_fOffectGridX = -fWidth / 2.0f;
                    tGridAngle1.m_fOffectGridY = fWidth / 2 - fEdgeWidth / 2.0f;
                    break;
                }
                nGridBit = nGridBit - 8;
                if (nGridBit - 4 < 0) // 左上
                {
                    if (nGridBit - 2 < 0) // 上
                    {
                        tGridAngle1 = new GridEdgeOffect(this);
                        tGridAngle1.m_tEdgeId = grid_line;
                        tGridAngle1.m_fOffectGridX = -fWidth / 2.0f;
                        tGridAngle1.m_fOffectGridY = fWidth / 2 + fEdgeWidth / 2.0f;
                        break;
                    }
                    // 左下和右上
                    tGridAngle1 = new GridEdgeOffect(this);
                    tGridAngle1.m_tEdgeId = grid_floorInside;
                    tGridAngle1.m_nAngle = 270; // 左上
                    tGridAngle1.m_fOffectGridX = -fWidth / 2.0f - fInSideWidth / 2;
                    tGridAngle1.m_fOffectGridY = fWidth / 2 + fInSideWidth / 2;
                    tGridAngle2 = new GridEdgeOffect(this);
                    tGridAngle2.m_tEdgeId = grid_floorInside;
                    tGridAngle2.m_nAngle = 90; // 右下
                    tGridAngle2.m_fOffectGridX = -fWidth / 2.0f + fInSideWidth / 2;
                    tGridAngle2.m_fOffectGridY = fWidth / 2 - fInSideWidth / 2;
                    break;
                }
                /// 右侧
                tGridAngle1 = new GridEdgeOffect(this);
                tGridAngle1.m_tEdgeId = grid_line;
                tGridAngle1.m_nAngle = 270;
                tGridAngle1.m_fOffectGridX = -fWidth / 2.0f + fEdgeWidth / 2.0f;
                tGridAngle1.m_fOffectGridY = fWidth / 2;
                break;
            }
            if (tGridAngle1 != null)
            {
                tGridAngle1.copyAndSetInSide(fPosX, fPosY);
            }
            if (tGridAngle2 != null)
            {
                tGridAngle2.copyAndSetInSide(fPosX, fPosY);
            }
        }

        void battle_set1GridAnglePosition(int nGridBit, float fPosX, float fPosY)
        {
            /** inside grid edge **/
            GridEdgeOffect tGrid3Angle = new GridEdgeOffect(this);
            while (true)
            {
                float fWidth = battleGridWidth;
                float fEdgeWidth = battleEdgeWidth;
                float fOutSideWidth = battleEdgeOutSideWidth;

                tGrid3Angle.m_tEdgeId = grid_floorOutSide;
                if (nGridBit == 8) // 左边
                {
                    tGrid3Angle.m_nAngle = 270;
                    tGrid3Angle.m_fOffectGridX = -fWidth / 2.0f - fOutSideWidth / 2.0f + fEdgeWidth;
                    tGrid3Angle.m_fOffectGridY = fWidth / 2 - fOutSideWidth / 2.0f + fEdgeWidth;
                    break;
                }
                else if (nGridBit == 4) // 左上
                {
                    tGrid3Angle.m_nAngle = 180;
                    tGrid3Angle.m_fOffectGridX = -fWidth / 2.0f - fOutSideWidth / 2.0f + fEdgeWidth;
                    tGrid3Angle.m_fOffectGridY = fWidth / 2 + fOutSideWidth / 2.0f - fEdgeWidth;
                    break;
                }
                else if (nGridBit == 2) // 右上
                {
                    tGrid3Angle.m_nAngle = 90;
                    tGrid3Angle.m_fOffectGridX = -fWidth / 2.0f + fOutSideWidth / 2.0f - fEdgeWidth;
                    tGrid3Angle.m_fOffectGridY = fWidth / 2 + fOutSideWidth / 2.0f - fEdgeWidth;
                    break;
                }
                else
                {
                    tGrid3Angle.m_fOffectGridX = -fWidth / 2.0f + fOutSideWidth / 2.0f - fEdgeWidth;
                    tGrid3Angle.m_fOffectGridY = fWidth / 2 - fOutSideWidth / 2.0f + fEdgeWidth;
                }
                break;
            }
            tGrid3Angle.copyAndSetInSide(fPosX, fPosY);
        }
        public void battle_showGridEdge(ChessBoard tChessBoard, float fPosX, float fPosY, int nL, int nC)
        {
            // 区分当前本身 左 左上 上 的格子状态，获取当前用什么样的皮肤 4个格子
            /** 0或4个方块 什么都不做
            	3个方块 内角
            	2个方块 直线 直线 横竖
            			对角线 两个内角
            	1个方块 一个外角
            **/
            int nBeginLine = nL;
            int nBeginColumn = nC;
            Grid tGrid = tChessBoard.getGrid(nBeginLine, nBeginColumn);
            Grid tUpGrid = tChessBoard.getGrid(nBeginLine - 1, nBeginColumn);
            Grid tUpLeftGrid = tChessBoard.getGrid(nBeginLine - 1, nBeginColumn - 1);
            Grid tLeftGrid = tChessBoard.getGrid(nBeginLine, nBeginColumn - 1);
            int nGridBit = 0; // current 1  up 2  up left 4  left 8
            int nGridOkNum = 0;
            if (tGrid != null)
            {
                nGridBit = nGridBit + 1;
                nGridOkNum = nGridOkNum + 1;
            }
            if (tUpGrid != null)
            {
                nGridBit = nGridBit + 2;
                nGridOkNum = nGridOkNum + 1;
            }
            if (tUpLeftGrid != null)
            {
                nGridBit = nGridBit + 4;
                nGridOkNum = nGridOkNum + 1;
            }
            if (tLeftGrid != null)
            {
                nGridBit = nGridBit + 8;
                nGridOkNum = nGridOkNum + 1;
            }
            if (nGridOkNum <= 0 || nGridOkNum == 4)
            {
                return;
            }
            if (nGridOkNum == 3)
            {
                /** 如果是3个，则是内环 **/
                battle_set3GridAnglePosition(nGridBit, fPosX, fPosY);
            }
            else if (nGridOkNum == 2)
            {
                battle_set2GridAnglePosition(nGridBit, fPosX, fPosY);
            }
            else if (nGridOkNum == 1)
            {
                battle_set1GridAnglePosition(nGridBit, fPosX, fPosY);
            }
            return;
        }

        void show()
        {
            for (int nLine = 0; nLine <= m_tChessBoard.m_nHeight; nLine++)
            {
                for (int nCol = 0; nCol <= m_tChessBoard.m_nWidth; nCol++)
                {
                    Vector2 tVector2 = MoveUnitUtil.getPositionWithLineCol(nLine, nCol);
                    battle_showGridEdge(m_tChessBoard, tVector2.x, tVector2.y, nLine, nCol);
                }
            }
        }
    }
}