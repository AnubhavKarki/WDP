// 1. Home/Index.cshtml: Fade in and out
const containers = document.querySelectorAll(".image-background-container");

const options = {
  root: null,
  threshold: 1,
};

containers.forEach((container) => {
  const infoBlock = container.querySelector(".info-block");
  const imageContainer = container.querySelector(".image-container");

  const observer = new IntersectionObserver((entries) => {
    let isVisible = false;

    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        isVisible = true;
      }
    });

    if (isVisible) {
      if (infoBlock) {
        infoBlock.classList.remove("fade-out");
        infoBlock.classList.add("fade-in");
      }

      if (imageContainer) {
        imageContainer.classList.remove("fade-out");
        imageContainer.classList.add("fade-in");
      }
    } else {
      if (infoBlock) {
        infoBlock.classList.remove("fade-in");
        infoBlock.classList.add("fade-out");
      }

      if (imageContainer) {
        imageContainer.classList.remove("fade-in");
        imageContainer.classList.add("fade-out");
      }
    }
  }, options);

  if (infoBlock) {
    observer.observe(infoBlock);
  }

  if (imageContainer) {
    observer.observe(imageContainer);
  }

  if (infoBlock) {
    infoBlock.classList.add("fade-out");
  }

  if (imageContainer) {
    imageContainer.classList.add("fade-out");
  }
});

// 2. AIImages/Index.cshtml:
function likeAndAnchor(imageId) {
  localStorage.setItem("likedImageId", imageId);
  return true;
}

document.addEventListener("DOMContentLoaded", function () {
  var likedImageId = localStorage.getItem("likedImageId");
  if (likedImageId) {
    var element = document.getElementById("image-" + likedImageId);
    if (element) {
      element.scrollIntoView({ behavior: "smooth" });
    }
    localStorage.removeItem("likedImageId");
  }
});

// 3. AIImages/Edit.cshtml:
function previewImage(event) {
  const input = event.target;
  const file = input.files[0];
  const currentImage = document.getElementById("currentImage");
  const preview = document.getElementById("selectedImage_");

  if (file) {
    const reader = new FileReader();
    reader.onload = function (e) {
      // Temporarily replace the current image with the new one
      preview.src = e.target.result;
      preview.style.display = "block";

      // Optionally hide the current image
      currentImage.style.display = "none";
    };
    reader.readAsDataURL(file);
  } else {
    // If no file is selected, show the current image again
    preview.style.display = "none";
    currentImage.style.display = "block";
  }
}

// 4. AIImages/Create.cshtml:
document.addEventListener("DOMContentLoaded", function () {
  const imageUploadInput = document.getElementById("imageUpload");
  const preview = document.getElementById("selectedImage");

  if (imageUploadInput) {
    imageUploadInput.addEventListener("change", previewImage_);
  }

  function previewImage_(event) {
    const input = event.target;
    const file = input.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = function (e) {
        preview.src = e.target.result;
      };
      reader.readAsDataURL(file);
    } else {
      preview.src = "~/images/ibm.jpg"; 
    }
  }
});
