using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementNotice : MonoBehaviour
{
    // Start is called before the first frame update
    ENate.Element m_tElement;
    jc.EventManager.EventObj m_tEventObj;
    string m_strReputationId;
    public GameObject num;
    void Start()
    {
        m_tElement = transform.parent.GetComponent<ENate.Element>();
        m_tEventObj = new jc.EventManager.EventObj();
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_REPUTATION_CHANGE, event_ReputationChange);
        ConditionConfig.MapArg mpArg = new ConditionConfig.MapArg();
        mpArg.Element = m_tElement;
        if (m_tElement != null && m_tElement.m_tGrid != null)
        {
            mpArg.ChessBoard = m_tElement.m_tGrid.m_tChessBoard;
        }
        ENate.ElementValue<string> tReputation = m_tElement.getElementAttribute(ENate.ElementAttribute.Attribute.reputationId) as ENate.ElementValue<string>;
        if (tReputation == null)
        {
            return;
        }
        else
        {
            m_strReputationId = ConditionConfig.Reputation.getReputationId(tReputation.Value, mpArg);
            num.setTextParam(ConditionConfig.Reputation.get(m_strReputationId).ToString());
        }
    }

    void event_ReputationChange(object o)
    {
        KeyValuePair<string, int> tKeyValue = (KeyValuePair<string, int>) o;
        if (tKeyValue.Key == m_strReputationId)
        {
            num.setTextParam(tKeyValue.Value.ToString());
        }
    }
    private void OnDestroy()
    {
        if (m_tEventObj == null)
        {
            return;
        }
        m_tEventObj.clear();
        m_tEventObj = null;
    }

}