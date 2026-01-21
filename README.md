
## Niên luận ngành: *Phát triển game 2D platform trên Unity*
>Logo: Updating ...
>Giới thiệu Game: Updating ...
## Cấu trúc cây thư mục
* Asset/  
  *  Animation/: Chứa Animator và Animation của các đối tượng trong game theo từng thư mục.  
  *  Audio/: Chứa các file âm thanh  
      *  Background Music/: Chứa nhạc nền của trò chơi.  
      *  UI SFX/: Chứa các hiệu ứng âm thanh khi tương tác với UI.  
  		*  Gameplay SFX/: Chứa các hiệu ứng âm thanh khi chơi trò chơi.  
  *  Art/: Chứa các hình ảnh, sprite sheet trong trò chơi.  
      *  Characters/: Chứa các hình ảnh của nhân vật và các NPC.  
      *  Enemies/: Chứa các hình ảnh của kẻ thù  
      *  Environment/: Chứa các hình ảnh, sprite sheet liên quan đến môi trường.  
  *  Fonts/: Chứa các font chữ được dùng trong dự án.  
  *  UI/: Chứa các hình ảnh của UI trò chơi.  
  *  Prefabs/: Chứa các prefab trong trò chơi.  
  *  Scripts/: Chứa script của dự án.  
      *  Player/: Chứa script liên quan đến các thao tác, hành vi của người chơi.  
      *   UI/: Chứa script liên quan đến quản lý giao diện.  
   *  Enemies/: Chứa script liên quan đến các thao tác, hành vi của quái vật.  
  *  Palettes/: Chứa các tile palette của trò chơi.  
  *  Scenes/: Chứa các scene có trong trò chơi  
  *  Tiles/: Chứa các tile  của trò chơi.  
## Thông tin người chơi 
***Người chơi***
Máu tối đa: 10
Stamina tối đa: 100
| Hành động | Mô tả | Điều kiện | Ảnh |
|--|--|--|--|
| Di chuyển | Người chơi di chuyển theo trục x với tốc độ 5 đơn vị mỗi frame | Ấn phím A, D | Updating ... |
| Leo thang | Người chơi di chuyển theo trục y với tốc độ 8 đơn vị mỗi frame | Va chạm với thang và ấn phím W, S |  |
| Nhảy | Người chơi bật lên cao với tốc độ 22 đơn vị mỗi frame, hỗ trợ Coyote Time (0.1s) | Ấn phím Space và đang ở trên mặt đất |  |
| Nhảy trên không | Cho phép nhảy một lần nữa khi không va chạm với mặt đất | Ấn phím Space và đang ở trên không |  |
| Nhảy trên tường | Người chơi nhảy qua lại giữa các bức tường theo hướng chéo ngược lại với hướng đang nhìn với tốc độ 25 đơn vị mỗi frame | Ấn phím Space và đã va chạm với tường |  |
| Trượt tường | Nhân vật di chuyển chậm lại với tốc độ 2 đơn vị mỗi giây khi đang trên trên không và va chạm với tường | Ấn A hoặc D (theo vị trí của tường gần nhất) và ở trên không|  |
| Lướt | Người chơi di chuyển với tốc độ 5 đơn vị mỗi frame trong khoảng thời gian 0.2s, đợi 0.35s giữa mỗi lần dash  | Nhấn Shift trên mặt đất hoặc trên không |  |
| Tấn công mặt đất (x3) | Người chơi tấn công 3 đoạn (3x combo), sát thương ở đoạn 1 là 2, đoạn 2 là 2x1.2, đoạn 3 là 2x1.5  | Ấn chuột trái liên tục, khoảng cách giữa các lần ấn bé hơn 0.8s để tăng số đoạn của combo |  |
| Tấn công trên không (x1) | Người chơi tấn công 1 lần trên không với sát thương là x | Ấn chuột trái trên không |  |
| Bắn | Người chơi thực hiện đòn đánh tầm xa trong khoảng cách x, sát thương x, khoảng cách giữa các lần bắn là x giây | Ấn chuột phải |  |
| Stamina | Khi lướt sẽ tiêu hao 50 stamina và tự động hồi lại với thời gian 1 stamina mỗi frame | Tự động hồi |  |
| Hồi máu | Người chơi được hồi lại 2 máu và tiêu hao 1 bình máu, tiêu hao 3 máu, thời gian animation uống bình là 1.2s, thời gian hồi lại bình máu là 2s  | Ấn F khi đứng yên |  |
| Nhận sát thương | Người chơi sẽ bị đánh văng ra (knockback) khi nhận bất kì sát thương với lực theo trục X là 6, trục Y là 2 trong 0.2s và kích hoạt iframe trong 0.2s | Khi nhận sát thương bất kì |  |
| Chết | Hoạt ảnh chết khi máu người chơi về 0 và hồi sinh tại điểm lưu gần nhất | Khi máu người chơi về 0 |  |
| Tương tác vật phẩm, NPC | Updating ... |  |  |

## Thông tin các loại quái vật
***Quái vật***
| Tên | Thông tin | Mô tả | Ảnh |
|--|--|--|--|
| Mộc tinh |  |  |  |
| Ma da |  |  |  |
| Quỷ Nhập Tràng |  |  |  |
| Ông Năm chèo (BOSS) |  |  |  |
