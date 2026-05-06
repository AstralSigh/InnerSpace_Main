using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInputs : MonoBehaviour
{
    public enum InputName{
        none,
        handInSphere_Right,
        handInSphere_Left,
        rightTrigger,
        leftTrigger,
        orbSelected_1SecHold_Right,
        orbSelected_1SecHold_Left,
        leftThumbstick,
        arrivedAtCircle,
        rightThumbstick,
        infoNodeGrabbed,
        infoNodeClosed,
        selectionByLaserPointer,
        dataPanelSelected,
        infoNodePanelSelected,
        teleportToInfoNode,
        timeWheelOpened,
        timeWheelPlayed,
        timeWheelScrubbed,
        triggerButton,
        completeTutorial,
        anyButton,
        rightGrip,
        leftGrip,
        skip,
        skipOneBeat,
        infoNodeReleased,
        infoNodePlusButton,
    }

    public enum TooltipName
    {
        leftController, leftThumbStick, leftTrigger,
        rightController, rightThumbstick, rightTrigger,
        rightGrip, leftGrip,
        none
    }

    public enum TPPivot
    {
        floatingPoint, laser01, laser2, laser3, timewheel, datawheel
    }

    //Fill Object references for these enums at InputManager in the TutorialManager GameObject
    public enum ObjectName
    {
        handInSphere_Right,
        handInSphere_Left,
    handInSphereHOLD_Right,
        handInSphereHOLD_Left,
        laserPractice01,
        laserPractice02,
        walkPractice01,
        walkPractice02,
        player,
        flyPractice01,
        flyPractice02,
        questControllerLeft,
        handLeft,
        questControllerRight,
        handRight,
        handInSphereAtFly02,
        infoNode,
        constituent,
        wristDockLeft,
        wristDockRight,
        nexusMapTooltip,
        tutorialScene,
        textPanel

    }


}
