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

Editor açılınca bir karşılama penceresi çıkar. Bir daha çıkmasın istiyorsan `Assets/Scripts/Editor/Window/VertigoCaseWelcomeWindow.cs` dosyasını sil.
