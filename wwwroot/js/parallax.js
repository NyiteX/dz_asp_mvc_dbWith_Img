/*$('.img-parallax').each(function() {
  var img = $(this);
  var imgParent = $(this).parent();

  function parallaxImg(event) {
    var speed = img.data('speed') * 0.4;
    var imgY = imgParent.offset().top;
    var winY = event.clientY;
    var winH = $(window).height();
    var parentH = imgParent.innerHeight();

    var winBottom = winY + winH;
    if (winBottom > imgY && winY < imgY + parentH) {
      var imgBottom = ((winBottom - imgY) * speed);
      var imgTop = winH + parentH;
      var imgPercent = ((imgBottom / imgTop) * 180) + (50 - (speed * 50));

      img.css({
        top: imgPercent + '%',
        transform: 'translate(-50%, -' + imgPercent + '%)'
      });
    }
  }

  $(document).on('mousemove', function(event) {
    parallaxImg(event);
  });

  $(window).on('resize', function(event) {
    parallaxImg(event);
  });
});*/

$('.img-parallax').each(function () {
    var img = $(this);
    var imgParent = $(this).parent();

    function parallaxImg() {
        var speed = img.data('speed') * 0.4;
        var imgY = imgParent.offset().top;
        var winY = $(window).scrollTop();
        var winH = $(window).height();
        var parentH = imgParent.innerHeight();

        var winBottom = winY + winH;
        if (winBottom > imgY && winY < imgY + parentH) {
            var imgBottom = ((winBottom - imgY) * speed);
            var imgTop = winH + parentH;
            var imgPercent = ((imgBottom / imgTop) * 180) + (50 - (speed * 50));

            img.css({
                top: imgPercent + '%',
                transform: 'translate(-50%, -' + imgPercent + '%)'
            });
        }
    }

    $(document).on('scroll', function () {
        parallaxImg();
    });

    $(window).on('resize', function () {
        parallaxImg();
    });
});
