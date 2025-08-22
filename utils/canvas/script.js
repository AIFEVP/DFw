document.addEventListener('DOMContentLoaded', function() {
  const captureDiv = document.getElementById('capture');
  const captureBtn = document.getElementById('capture-btn');
  const outputContainer = document.getElementById('output-container');

  captureBtn.addEventListener('click', function() {
    html2canvas(captureDiv).then(function(canvas) {
      // Create an image from the canvas
      const img = new Image();
      img.src = canvas.toDataURL('image/png');
      
      // Append the image to the output container
      outputContainer.innerHTML = ''; // Clear previous content
      outputContainer.appendChild(img);
      
      console.log('Capture successful!');
    }).catch(function(error) {
      console.error('oops, something went wrong!', error);
    });
  });
});