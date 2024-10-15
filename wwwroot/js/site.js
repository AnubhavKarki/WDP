const blocks = document.querySelectorAll(
  ".info-block, .img-fluid, .img-link, .upload-ai-section"
);

const options = {
  root: null,
  threshold: 1,
};

const observer = new IntersectionObserver((entries) => {
  entries.forEach((entry) => {
    if (entry.isIntersecting) {
      entry.target.classList.remove("fade-out");
      entry.target.classList.add("fade-in");
    } else {
      entry.target.classList.remove("fade-in");
      entry.target.classList.add("fade-out");
    }
  });
}, options);

blocks.forEach((block) => {
  block.classList.add("fade-out");
  observer.observe(block);
});
