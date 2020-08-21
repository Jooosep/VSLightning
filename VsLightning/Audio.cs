using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsLightning
{

    
    public class Audio
    {
        private string fp = Environment.CurrentDirectory;
        private System.Windows.Media.MediaPlayer endBGM = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer menuBGM = new System.Windows.Media.MediaPlayer();
        private List<System.Windows.Media.MediaPlayer> absorbs;
        private int absorbIt;

        private System.Windows.Media.MediaPlayer stormBG = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer shock1 = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer shock2 = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer shock3 = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer treeExplosion = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer finalRound = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer playerHit = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer nomSound = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer plantingSound = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer orbLaunch = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer orbAbsorb = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer orbHit = new System.Windows.Media.MediaPlayer();


        //Logos
        private System.Windows.Media.MediaPlayer chargeUp = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer shocker = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer highVoltage = new System.Windows.Media.MediaPlayer();

        public Audio()
        {
            absorbIt = 0;
            absorbs = new List<System.Windows.Media.MediaPlayer>();
            for (int i = 0; i < 2; i++)
            {
                absorbs.Add(new System.Windows.Media.MediaPlayer());
                absorbs[i].Open(new System.Uri(@"" + fp + "./absorb" + (i + 1) + ".wav"));
                absorbs[i].Volume = 0.7;
            }

            treeExplosion.Open(new System.Uri(@"" + fp + "./explosion.wav"));
            finalRound.Open(new System.Uri(@"" + fp + "./finalRound.wav"));
            playerHit.Open(new System.Uri(@"" + fp + "./shocker.wav"));

            shock1.Open(new System.Uri(@"" + fp + "./strike1.wav"));
            shock2.Open(new System.Uri(@"" + fp + "./strike2.wav"));
            shock3.Open(new System.Uri(@"" + fp + "./strike3.wav"));
            nomSound.Open(new System.Uri(@"" + fp + "./Rise02.wav"));
            plantingSound.Open(new System.Uri(@"" + fp + "./planting.wav"));
            endBGM.Open(new System.Uri(@"" + fp + "./BGM.wav"));
            menuBGM.Open(new System.Uri(@"" + fp + "./BG.wav"));
            stormBG.Open(new System.Uri(@"" + fp + "./BGStorm.wav"));
            orbLaunch.Open(new System.Uri(@"" + fp + "./orbLaunch.wav"));
            orbAbsorb.Open(new System.Uri(@"" + fp + "./orbAbsorb.wav"));
            orbHit.Open(new System.Uri(@"" + fp + "./orbHit.wav"));

            chargeUp.Open(new System.Uri(@"" + fp + "./chargeUp.wav"));
            shocker.Open(new System.Uri(@"" + fp + "./shocker.wav"));
            highVoltage.Open(new System.Uri(@"" + fp + "./highVoltage.wav"));
            highVoltage.Volume = 0.7;
            chargeUp.Volume = 1.0;
            shocker.Volume = 0.2;
            stormBG.Volume = 0.3;
            treeExplosion.Volume = 0.25;
            nomSound.Volume = 0.3;
            plantingSound.Volume = 0.3;
            orbLaunch.Volume = 0.8;
            playerHit.Volume = 0.4;

        }
        public void PlayHighVoltage()
        {
            highVoltage.Position = TimeSpan.Zero;
            highVoltage.Play();
        }
        public void PlayChargeUp()
        {
            chargeUp.Position = TimeSpan.Zero;
            chargeUp.Play();
        }
        public void StopChargeUp()
        {
            chargeUp.Stop();
        }
        public void PlayShocker()
        {
            shocker.Position = TimeSpan.Zero;
            shocker.Play();
        }
        public void PlayOrbHit()
        {
            orbHit.Position = TimeSpan.Zero;
            orbHit.Play();
        }
        public void PlayOrbLaunch()
        {
            orbLaunch.Position = TimeSpan.Zero;
            orbLaunch.Play();
        }
        public void PlayOrbAbsorb()
        {
            orbAbsorb.Position = TimeSpan.Zero;
            orbAbsorb.Play();
        }
        public void PlayPlantingSound()
        {
            plantingSound.Position = TimeSpan.Zero;
            plantingSound.Play();
        }
        public void PlayNom()
        {
            nomSound.Position = TimeSpan.Zero;
            nomSound.Play();
        }
        public void PlayStormBG()
        {
            stormBG.Position = TimeSpan.Zero;
            stormBG.Play();
        }
        public void PauseStormBG()
        {
            stormBG.Pause();
        }
        public void PlayEndBGM()
        {
            endBGM.Position = TimeSpan.Zero;
            endBGM.Play();
        }
        public void StopEndBGM()
        {
            endBGM.Stop();
        }
        public void ContinueStormBG()
        {
            stormBG.Play();
        }
        public void PlayAbsorb()
        {
            absorbs[absorbIt].Position = TimeSpan.Zero;
            absorbs[absorbIt].Play();
            absorbIt++;
            if(absorbIt == absorbs.Count)
            {
                absorbIt = 0;
            }
        }
        public void PlayShock1()
        {
            shock1.Position = TimeSpan.Zero;
            shock1.Play();
        }
        public void PlayShock2()
        {
            shock2.Position = TimeSpan.Zero;
            shock2.Play();
        }
        public void PlayShock3()
        {
            shock3.Position = TimeSpan.Zero;
            shock3.Play();
        }

        public void PlayExplosion()
        {
            treeExplosion.Position = TimeSpan.Zero;
            treeExplosion.Play();
        }
        public void PlayPlayerHit()
        {
            playerHit.Position = TimeSpan.Zero;
            playerHit.Play();
        }

        public void PlayMenuBGM()
        {
            menuBGM.Position = TimeSpan.Zero;
            menuBGM.Play();
        }
        public void StopMenuBGM()
        {
            menuBGM.Stop();
        }
    }
}
