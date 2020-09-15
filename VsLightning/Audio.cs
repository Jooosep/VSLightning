using NAudio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsLightning
{

    
    public class Audio
    {
        private static string fp = Environment.CurrentDirectory;
        private NAudio.Vorbis.VorbisWaveReader endBGMReader = new NAudio.Vorbis.VorbisWaveReader(fp + "./BGM.ogg");
        private NAudio.Vorbis.VorbisWaveReader menuBGMReader = new NAudio.Vorbis.VorbisWaveReader(fp + "./menuBGM.ogg");

        private List<NAudio.Vorbis.VorbisWaveReader> absorbReaders;
        private List<NAudio.Vorbis.VorbisWaveReader> shockerReaders;
        private int absorbIt;
        private int shockerIt;

        private NAudio.Vorbis.VorbisWaveReader stormBGReader = new NAudio.Vorbis.VorbisWaveReader(fp + "./BGStorm.ogg");
        private NAudio.Vorbis.VorbisWaveReader shock1Reader = new NAudio.Vorbis.VorbisWaveReader(fp + "./strike1.ogg");
        private NAudio.Vorbis.VorbisWaveReader shock2Reader = new NAudio.Vorbis.VorbisWaveReader(fp + "./strike2.ogg");
        private NAudio.Vorbis.VorbisWaveReader shock3Reader = new NAudio.Vorbis.VorbisWaveReader(fp + "./strike3.ogg");
        private NAudio.Vorbis.VorbisWaveReader orbLaunchReader = new NAudio.Vorbis.VorbisWaveReader(fp + "./orbLaunch.ogg");
        private NAudio.Vorbis.VorbisWaveReader orbAbsorbReader = new NAudio.Vorbis.VorbisWaveReader(fp + "./orbAbsorb.ogg");
        private NAudio.Vorbis.VorbisWaveReader orbHitReader = new NAudio.Vorbis.VorbisWaveReader(fp + "./orbHit.ogg");

        //Logos
        private NAudio.Vorbis.VorbisWaveReader chargeUpReader = new NAudio.Vorbis.VorbisWaveReader(fp + "./chargeUp.ogg");
        private NAudio.Vorbis.VorbisWaveReader shockerReader = new NAudio.Vorbis.VorbisWaveReader(fp + "./shocker.ogg");
        private NAudio.Vorbis.VorbisWaveReader highVoltageReader = new NAudio.Vorbis.VorbisWaveReader(fp + "./highVoltage.ogg");

        private NAudio.Wave.WaveOutEvent menuBGM = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent endBGM = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent shock1 = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent shock2 = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent shock3 = new NAudio.Wave.WaveOutEvent();

        private NAudio.Wave.WaveOutEvent highVoltage = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent chargeUp = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent shocker = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent stormBG = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent orbLaunch = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent orbHit = new NAudio.Wave.WaveOutEvent();
        private NAudio.Wave.WaveOutEvent orbAbsorb = new NAudio.Wave.WaveOutEvent();
        private List<NAudio.Wave.WaveOutEvent> absorbs;
        private List<NAudio.Wave.WaveOutEvent> shockers;

        private System.Windows.Media.MediaPlayer treeExplosion = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer nomSound = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer plantingSound = new System.Windows.Media.MediaPlayer();

        public Audio()
        {
            absorbReaders = new List<NAudio.Vorbis.VorbisWaveReader>();
            absorbs = new List<NAudio.Wave.WaveOutEvent>();
            for (int i = 0; i < 2; i++)
            {
                absorbReaders.Add(new NAudio.Vorbis.VorbisWaveReader(fp + "./absorb" + ((i % 2) + 1) + ".ogg"));
                absorbs.Add(new NAudio.Wave.WaveOutEvent());
                absorbs[i].Init(absorbReaders[i]);
            }

            shockerReaders = new List<NAudio.Vorbis.VorbisWaveReader>();
            shockers = new List<NAudio.Wave.WaveOutEvent>();
            for (int i = 0; i < 2; i++)
            {
                shockerReaders.Add(new NAudio.Vorbis.VorbisWaveReader(fp + "./shocker.ogg"));
                shockers.Add(new NAudio.Wave.WaveOutEvent());
                shockers[i].Init(shockerReaders[i]);
            }
            treeExplosion.Open(new System.Uri(@"" + fp + "./explosion.wav"));
            nomSound.Open(new System.Uri(@"" + fp + "./Rise02.wav"));
            plantingSound.Open(new System.Uri(@"" + fp + "./planting.wav"));

            shock1.Init(shock1Reader);  
            shock2.Init(shock2Reader);
            shock3.Init(shock3Reader);
            highVoltage.Init(highVoltageReader);
            chargeUp.Init(chargeUpReader);
            shocker.Init(shockerReader);
            stormBG.Init(stormBGReader);
            endBGM.Init(endBGMReader);
            menuBGM.Init(menuBGMReader);

            treeExplosion.Volume = 0.25f;

            nomSound.Volume = 0.3f;

            plantingSound.Volume = 0.3f;
            orbLaunch.Init(orbLaunchReader);
            orbLaunch.Volume = 0.5f;

            

        }
        public void PlayHighVoltage()
        {
            highVoltageReader.Position = 0;
            highVoltage.Play();
        }
        public void PlayStormBG()
        {
            stormBG.Volume = 0.5f;
            stormBGReader.Position = 0;
            stormBG.Play();
        }
        public void PauseStormBG()
        {
            stormBG.Pause();
        }
        public void StopStormBG()
        {
            stormBG.Stop();
        }
        public void ContinueStormBG()
        {
            stormBG.Play();
        }
        
        public void PlayChargeUp()
        {
            chargeUpReader.Position = 0;
            chargeUp.Play();
        }
        public void StopChargeUp()
        {
            chargeUp.Stop();
        }
        public void PlayShocker()
        {

            for (int i = 0; i < shockers.Count; i++)
            {
                if (shockers[i].PlaybackState == NAudio.Wave.PlaybackState.Stopped)
                {
                    shockerReaders[i].Position = 0;
                    shockers[i].Play();
                    return;
                }

            }

        }
        public void PlayOrbHit()
        {
            orbHitReader.Position = 0;
            orbHit.Play();
        }
        public void PlayOrbLaunch()
        {
            orbLaunchReader.Position = 0;
            orbLaunch.Play();
        }
        public void PlayOrbAbsorb()
        {
            orbAbsorbReader.Position = 0;
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
        
        public void PlayEndBGM()
        {
            endBGMReader.Position = 0;
            endBGM.Play();
        }
        public void StopEndBGM()
        {
            endBGM.Stop();
        }

        public void PlayAbsorb()
        {
            for (int i = 0; i < absorbs.Count; i++)
            {
                if (absorbs[i].PlaybackState == NAudio.Wave.PlaybackState.Stopped)
                {
                    absorbReaders[i].Position = 0;
                    absorbs[i].Play();
                    return;
                }

            }
            /*
            absorbReaders[absorbIt].Position = 0;
            absorbs[absorbIt].Play();
            absorbIt++;
            if(absorbIt == absorbs.Count)
            {
                absorbIt = 0;
            }
            */
        }
        public void PlayShock1()
        {
            shock1Reader.Position = 0;
            shock1.Play();
        }
        public void PlayShock2()
        {
            shock2Reader.Position = 0;
            shock2.Play();
        }
        public void PlayShock3()
        {
            shock3Reader.Position = 0;
            shock3.Play();
        }

        public void PlayExplosion()
        {
            treeExplosion.Position = TimeSpan.Zero;
            treeExplosion.Play();
        }

        public void PlayMenuBGM()
        {
            menuBGMReader.Position = 0;
            menuBGM.Play();
        }
        public void StopMenuBGM()
        {
            menuBGM.Stop();
        }
        
    }
}
