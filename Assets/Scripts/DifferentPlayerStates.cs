using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DifferentPlayerStates : MonoBehaviour
{

}


public class NoInput : PlayerState
{
    public NoInput(PlayerStateManager theGameStateManager) : base(theGameStateManager)
    {

    }

    public override void stateBehavior()
    {
        if (Input.GetKeyDown(KeyCode.Space)|| (Input.touches.Length != 0 && Input.touches[0].phase == TouchPhase.Began))
        {
            playerStateManager.ChangeState(new Dash(playerStateManager));
        }
        else if (playerStateManager.currentPlatform.GetComponentInParent<PlatformController>().platformDisappear == true)
        {
            playerStateManager.ChangeState(new Gameover(playerStateManager));
        }
        playerStateManager.BackGroundParticleRotate();
        playerStateManager.CameraRotate();
    }

    public override void Enter()
    {
        base.Enter();
        playerStateManager.PlayerStartRotate();
        playerStateManager.StartPlatformDisappear();

    }
    public override void Leave()
    {
        base.Leave();
        playerStateManager.PlayerStopRotate();
    }
}
public class Dash : PlayerState
{
    public Dash(PlayerStateManager theGameStateManager) : base(theGameStateManager)
    {

    }

    public override void stateBehavior()
    {
        if (PlayerStateManager.speed == 0||Input.GetKeyUp(KeyCode.Space)|| (Input.touches.Length != 0 && Input.touches[0].phase == TouchPhase.Ended))
        {
            playerStateManager.TransformBackToNormal();
            playerStateManager.ChangeState(new Stop(playerStateManager));
        }
        playerStateManager.ObjectScale();
        playerStateManager.BackGroundParticleRotate();
    }

    public override void Enter()
    {
        base.Enter();
        playerStateManager.ShortDash();

    }
    public override void Leave()
    {
        base.Leave();
        playerStateManager.StopDash();
        PlayerStateManager.speed = 0;
        
    }
}

public class Stop : PlayerState
{
    int a = 0;
    public Stop(PlayerStateManager theGameStateManager) : base(theGameStateManager)
    {

    }

    public override void stateBehavior()
    {
        if (playerStateManager.cameraFollowCoroutine == null)
        {
            playerStateManager.ChangeState(new NoInput(playerStateManager));
        }
        playerStateManager.TransformBackToNormal();
        playerStateManager.BackGroundParticleStop();
    }

    public override void Enter()
    {
        base.Enter();
        playerStateManager.CameraStartFollow();
        playerStateManager.TransformBackToNormal();
        a = playerStateManager.CheckCollision();
        //check overlaps with the platform
        if (a == 0)
        {
            //Debug.Log("-life");
            playerStateManager.BackGroundParticleTurnColor(playerStateManager.ColorWhenNotHitPlatform);

            playerStateManager.ChangeState(new Gameover(playerStateManager));
            //reset platform
            //reset transform
            //-life
        }
        else if (a == 1)
        {
            //spawn platform
            //despawn current platform
            playerStateManager.ActivatePlatform();
            
            playerStateManager.BackGroundParticleTurnColor(playerStateManager.ColorWhenHitPlatform);
        }
        else if (a == 2)
        {
           // playerStateManager.currentPlatform.GetComponentInParent<PlatformController>().SpawnPlatform(Random.Range(1, 4));
            //despawn current platform
            playerStateManager.ActivatePlatform();
            playerStateManager.BackGroundParticleTurnColor(playerStateManager.ColorWhenPerfectlyHitPlatform);
            playerStateManager.currentPlatform.GetComponentInParent<PlatformController>().HitPerfect = true;
        }

    }
    public override void Leave()
    {
        base.Leave();
        if (a>0)
        {
            playerStateManager.currentPlatform.GetComponentInParent<PlatformController>().SpawnPlatform(Random.Range(1, 4));
            
        }

    }
}

public class Gameover : PlayerState
{
    
    public Gameover(PlayerStateManager theGameStateManager) : base(theGameStateManager)
    {

    }

    public override void stateBehavior()
    {
        if (Input.GetKeyUp(KeyCode.Space)||(Input.touches.Length!=0 && Input.touches[0].phase == TouchPhase.Ended))
        {
            //Debug.Log("restart");
            SceneManager.LoadScene(0);
        }
    }

    public override void Enter()
    {
        base.Enter();
        playerStateManager.Gameover();

    }
    public override void Leave()
    {
        base.Leave();
        
    }
}

