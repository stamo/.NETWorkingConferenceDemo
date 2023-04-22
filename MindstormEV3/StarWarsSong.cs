using Lego.Mindstorms;

namespace MindstormEV3
{
    /// <summary>
    /// Star Wars song
    /// </summary>
    internal class StarWarsSong
    {
        // Notes
        const int c = 261;
        const int d = 294;
        const int e = 329;
        const int f = 349;
        const int g = 391;
        const int gS = 415;
        const int a = 440;
        const int aS = 455;
        const int b = 466;
        const int cH = 523;
        const int cSH = 554;
        const int dH = 587;
        const int dSH = 622;
        const int eH = 659;
        const int fH = 698;
        const int fSH = 740;
        const int gH = 784;
        const int gSH = 830;
        const int aH = 880;

        /// <summary>
        /// Mindstorm client
        /// </summary>
        private readonly MindstormsClient<BluetoothCommunication> _client;

        /// <summary>
        /// Creates new song instance
        /// </summary>
        /// <param name="client">Mindstorm client</param>
        public StarWarsSong(MindstormsClient<BluetoothCommunication> client)
        {
            _client = client;
        }

        /// <summary>
        /// Plays the song
        /// </summary>
        /// <returns></returns>
        internal async Task Play()
        {
            await FirstSection();
            await SecondSection();
            await Variant1();
            await SecondSection();
            await Variant2();
        }

        /// <summary>
        /// First section of the song
        /// </summary>
        /// <returns></returns>
        private async Task FirstSection()
        {
            await PlayNote(a, 500);
            await PlayNote(a, 500);
            await PlayNote(a, 500);
            await PlayNote(f, 350);
            await PlayNote(cH, 150);
            await PlayNote(a, 500);
            await PlayNote(f, 350);
            await PlayNote(cH, 150);
            await PlayNote(a, 650);
            Thread.Sleep(500);

            await PlayNote(eH, 500);
            await PlayNote(eH, 500);
            await PlayNote(eH, 500);
            await PlayNote(fH, 350);
            await PlayNote(cH, 150);
            await PlayNote(gS, 500);
            await PlayNote(f, 350);
            await PlayNote(cH, 150);
            await PlayNote(a, 650);

            Thread.Sleep(500);
        }

        /// <summary>
        /// Second section of the song
        /// </summary>
        /// <returns></returns>
        private async Task SecondSection()
        {
            await PlayNote(aH, 500);
            await PlayNote(a, 300);
            await PlayNote(a, 150);
            await PlayNote(aH, 500);
            await PlayNote(gSH, 325);
            await PlayNote(gH, 175);
            await PlayNote(fSH, 125);
            await PlayNote(fH, 125);
            await PlayNote(fSH, 250);

            Thread.Sleep(325);

            await PlayNote(aS, 250);
            await PlayNote(dSH, 500);
            await PlayNote(dH, 325);
            await PlayNote(cSH, 175);
            await PlayNote(cH, 125);
            await PlayNote(b, 125);
            await PlayNote(cH, 250);

            Thread.Sleep(350);
        }

        /// <summary>
        /// Additional section
        /// </summary>
        /// <returns></returns>
        private async Task Variant1()
        {
            await PlayNote(f, 250);
            await PlayNote(gS, 500);
            await PlayNote(f, 350);
            await PlayNote(a, 125);
            await PlayNote(cH, 500);
            await PlayNote(a, 375);
            await PlayNote(cH, 125);
            await PlayNote(eH, 650);

            Thread.Sleep(500);
        }

        /// <summary>
        /// Additional section
        /// </summary>
        /// <returns></returns>
        private async Task Variant2()
        {
            await PlayNote(f, 250);
            await PlayNote(gS, 500);
            await PlayNote(f, 375);
            await PlayNote(cH, 125);
            await PlayNote(a, 500);
            await PlayNote(f, 375);
            await PlayNote(cH, 125);
            await PlayNote(a, 650);

            Thread.Sleep(650);
        }

        /// <summary>
        /// Plays a note
        /// </summary>
        /// <param name="note">Frequency / Note</param>
        /// <param name="duration">Note duration</param>
        /// <returns></returns>
        private async Task PlayNote(ushort note, ushort duration)
        {
            await _client.PlayToneAsync(100, note, duration);
            Thread.Sleep(duration);
        }
    }
}
