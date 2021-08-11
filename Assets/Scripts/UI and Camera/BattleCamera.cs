using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera vCam;
    public Cinemachine.CinemachineTargetGroup targetGroup;

    // Start is called before the first frame update
    public void Init()
    {
        vCam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        targetGroup = FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
    }

    public void AddFocus(GameObject gameObject)
    {
        for (int i = 0; i < targetGroup.m_Targets.Length; i++)
        {
            if (targetGroup.m_Targets[i].target.name == gameObject.name)
            {
                targetGroup.m_Targets[i].weight += 1f;
                //targetGroup.m_Targets[i].radius = 1f;
            }
        }
    }

    public void ResetTargets()
    {
        for (int i = 0; i < targetGroup.m_Targets.Length; i++)
        {
            targetGroup.m_Targets[i].weight = 1;
        }
    }
    
    public void FocusTwo(GameObject attacker, List<GameObject> targets)
    {
        StartCoroutine(LookAtTwo(attacker, targets));
    }

    IEnumerator LookAtTwo(GameObject attacker, List<GameObject> targets)
    {
        AddFocus(attacker);
        yield return new WaitForSeconds(1f);

        foreach (GameObject target in targets)
        {
            AddFocus(target);
        }

        yield return new WaitForSeconds(2f);
        ResetTargets();
    }

    public void AddMember(GameObject member)
    {
        if (targetGroup.FindMember(member.transform) == -1)
        {
            targetGroup.AddMember(member.transform, 1, 1);
        }
    }
}
