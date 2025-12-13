$(document).ready(function () {
    $('.video-card').click(function () {        
        var videoId = $(this).data('video-id');
        var videoUrl = 'https://www.youtube.com/embed/' + videoId + '?autoplay=1&showinfo=0&rel=0&modestbranding=1';
        $('#youtubePlayer').attr('src', videoUrl);
        $('#videoModal').modal('show');
    });
    $('#videoModal').on('hidden.bs.modal', function () {
        $('#youtubePlayer').attr('src', '');
    });
});
