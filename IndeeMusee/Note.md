## Ghi chú cần làm
1. Lọc ra các trường dữ liệu của 1 bài hát
   2. Add tags to mp3
2. Tìm cách gán mã ID cho bài hát


3. Ghi chú: NowPlayingSong chỉ được UpNextModel truy cập để chuyển đổi bài hát đang phát

4. Chỉ quản lý bài hát đang trong queue
 bằng PlayingKey, không quản lý bằng index hay
 SongKey vì sẽ có nhiều bài hát trùng
 trong queue

dùng command trong list view item https://www.codeproject.com/Questions/673802/Select-Item-in-list-view-on-clicking-a-button-insi
command parameter ={binding} để truyền object vào sự kiện


5. LỖI CẦN CHỈNH SỬA

+ đổi font của playlist view (lỗi tiếng việt)
+ hình của createplaylist chưa nét
+ tăng margin top của play list card 
+ Đổi màu khi nhấp vào nút shuffle\
+ LÀm nồi bật các form lên
+ Thiếu danh sách bài hát trong playlist info views
+ thêm một sidebar lyric 
+ xem lại chức năng play again
+ xem lại chỗ nowplaying, nếu nhiều bài hát quá thì 
+ lỗi ui trong list music dialog,  nếu title quá dài thì sẽ bị phá cái grid, cần set cứng
+ ở now playing view, cái scroll viewer che cái nút remove luôn
+ add emty space when scroll end of list view 
+ đổi màu nút sidebbar khi chọn trang


** update 

1. thêm nút rename của playlist
2. thêm hình ảnh vào playlist
3. form add playlist (khang)
4. tên bài hát ở thanh taskbar, (hình ảnh - đĩa xoay)
5. đĩa xoay ở home, visuallize
6. đổi màu shuffle
7. loading khi mở chương trình ()
8. đặt tên cho phần mềm
9. bật form dialog phần đằng sau bị mờ đi (novapo)
10. nút space là play/ pause
11. form create folder bấm create ko chọn url lên bị lỗi
12. add tên bài trùng nhau thì ko cho
13. set tool tip cần thiết
14. giới hạn số lượng list view tree, 
15. bỏ luôn scrollbar ()
16. minitool kế bên search bar (đổi màu, đổi thư mục người dùng, thoát)
17. lỗi không edit được thông tin bài hát
18. design lại playlist info view
19. phần hẹn giờ