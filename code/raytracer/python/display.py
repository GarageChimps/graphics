from PIL import Image


def display(image_data, width, height):

    image_list = []
    for i in range(width):
        for j in range(height):
            pixelInBytes = [int(255 * x) for x in image_data[i][j]]
            image_list.append(tuple(pixelInBytes))

    img = Image.new('RGB', (width, height))
    img.putdata(image_list)
    img.save('image.png')

