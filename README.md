
# Vertigo Games Case

Critical Strike tarzı bir Spin Mode. Oyuncu çarkı çevirir, gelen ödülü kasasında biriktirir. Bomba gelirse ya devam eder ya da turu bırakır.

**Osman Turan Varol**

## Oynanış

1. Menüden Spin Mode’a gir.
2. Ücret ödeyerek başla. İstersen ileri bir zone’dan da girebilirsin.
3. Çarkı çevir. Ödül kasaya gider ve bir sonraki zone açılır. Ekrana dokunursan dönüş atlanır.
4. Bomba gelirse dilim kapanır. Revive ile aynı çarkta devam edersin. O dilim bir daha gelmez. Ücret zone ilerledikçe artar.
5. Güvenli bir zone’da çıkıp ödülleri alabilirsin. Son zone de geçilirse tur biter.
6. Alınan ödüller sonuç ekranında listelenir.

## Açmak

Unity **6000.3.23f1** ile `Assets/Scenes/DemoScene` sahnesini aç ve Play’e bas.

Çark, zone ve ödül ayarları `Assets/ScriptableObjects` altında.

- **Tools / Vertigo / Database Window** — eşyalar
- **Tools / Vertigo / Spin Database Window** — zone, çark ve ödül havuzları

<img width="1318" height="971" alt="DatabaseWindow" src="https://github.com/user-attachments/assets/b20bf151-9af8-4168-95c9-e6cc4893f1ea" />
<img width="1319" height="1235" alt="SpinDatabaseWindow" src="https://github.com/user-attachments/assets/553559d0-0f0e-422b-9bae-757e3dcc7e1d" />
<img width="1189" height="669" alt="DemoScene_1080p_28 10 2026_17-06-59" src="https://github.com/user-attachments/assets/3ae02b5c-73cd-43c8-8567-f472e1fe6df2" />
<img width="1189" height="669" alt="DemoScene_1080p_28 10 2026_17-09-28" src="https://github.com/user-attachments/assets/fd28b9a3-d3e8-4554-bfb7-73f0b4bde9cb" />

<img width="1189" height="669" alt="DemoScene_1080p_28 10 2026_17-06-14" src="https://github.com/user-attachments/assets/7519ba03-99f8-4750-8e01-ef1874ae40d7" />
<img width="1189" height="669" alt="DemoScene_1080p_28 10 2026_17-12-29" src="https://github.com/user-attachments/assets/ab383846-d242-48d4-89df-36d98690d21b" />

Editor açılınca bir karşılama penceresi çıkar. Bir daha çıkmasın istiyorsan `Assets/Scripts/Editor/Window/VertigoCaseWelcomeWindow.cs` dosyasını sil.
