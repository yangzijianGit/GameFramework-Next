/*
 * @Description: StageRunStaue
 * @Author: yangzijian
 * @Date: 2020-05-29 11:47:02
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-06-22 17:33:53
 */

using UnityEngine;
namespace ENate
{

    public enum StageRunningStatus
    {
        DetectingWin, // 检测胜利
        DetectingReset, // 检测重置
        ResetAniOverCheckWin, // 检测重置动画结束，检测胜利
        DetectingRoundEnd, // 检测是否进入回合结束
        RoundEnd, // 棋盘准备状态
        RoundEndOver, // 棋盘准备状态
        Calmness, // 平静
        Win, // 胜利
        MoveToNextChessBoard, // 胜利
        Carnical, // 结束之后播放动画状态
        Failed, // 失败
        Over, // 结束
        DetectingFever,
        Fever,
        ClothesSkill,
        UseHammer,
        num
    }
    public interface StageRunStaue
    {
        void prefix(ENate.Stage tStage);
        void run(ENate.Stage tStage);
        bool isOver(ENate.Stage tStage);
        ENate.StageRunningStatus end(ENate.Stage tStage);
    }
}