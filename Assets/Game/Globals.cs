using UnityEngine;

public enum Movable {
    No,
    Yes,
    SecondFloor
}

public enum Blowable {
    No,
    BlowStopper,
    Yes
}

public enum CellItem {
    Null,
    PlayerRespawn,
    EnemyRespawn
}

/**
 * Objects states
 */
public static class State {
    public static readonly int Default = 0;
    public static readonly int Awake = 1;
    public static readonly int GamePlay = 2;
    public static readonly int Editor = 3;

    public enum Global {
    }


    public enum Cube {
        MoveLeft,
        MoveRight
    }
}

/*
 * Radio channels (events)
 */
public static class Channel {
    public enum State {
        Enable,
        Set,
        Disable,
    }

    public enum GameObject {
        Awake,
        Start,
        Enable,
        Disable,
        Destroy
    }

    public enum Controller {
        SetState
    }

    public enum Map {
        Loading,
        Loaded,
        NewCell,
        SetCellParams,

        BlowCell,
        MakeBlowCell,
        SetCellItem,

        MoveBlockDown
    }

    public enum Actor {
        StartMove,
        FinishMove,
        Fire,
        ChangeLocation
    }
}

public static class g {
    public static readonly float FixedUpdateFrameRate = 1f / 30f; // default value == 0.02f

    public static readonly string MapPath = "Assets/Maps/";
    public static readonly int NextBlowDelay = 5; // Задержка перед взрывом следующей клетки

    public static GameController c;
    public static Camera camera;
    public static CameraStateController cameraController;
    public static MapController map;
}

public static class console {
    public static void log(params object[] args) {
        var str = "";
        foreach (var arg in args) {
            if (str != "") {
                str += ", ";
            }
            str += arg == null ? "NULL" : arg.ToString();
        }
        Debug.Log(str);
    }
}