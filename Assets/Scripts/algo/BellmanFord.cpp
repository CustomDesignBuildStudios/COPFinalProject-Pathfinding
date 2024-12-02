#include <SFML/Graphics.hpp>
#include <vector>
#include <random>
#include <limits>

// Constants to define cell size, grid dimensions, and solver speed
const int CELL_SIZE = 20;             // Size of each cell in pixels
const int GRID_WIDTH = 30;            // Number of cells in the grid width
const int GRID_HEIGHT = 30;           // Number of cells in the grid height
const float SOLVE_SPEED = 0.05f;      // Time delay (in seconds) for each solve step

/**
 * Represents a single cell in the grid. Each cell can be a wall, visited,
 * or have a specific distance and a previous cell for path tracking.
 */
struct Cell {
    bool isWall = false;
    int distance = std::numeric_limits<int>::max();
    sf::Vector2i previous = { -1, -1 };
    bool visited = false;
};

/**
 * Comparison operator for sf::Vector2i to check if two vectors are equal.
 */
bool operator==(const sf::Vector2i& left, const sf::Vector2i& right) {
    return left.x == right.x && left.y == right.y;
}

class MazeSolver {
private:
    std::vector<std::vector<Cell>> grid;  // 2D grid of cells
    sf::RenderWindow window;              // SFML window for rendering
    sf::Vector2i start;                   // Start position of the maze
    sf::Vector2i end;                     // End position of the maze
    bool solving = false;                 // Flag to indicate if the maze is being solved
    sf::Clock clock;                      // Timer for controlling solve speed

    /**
     * Checks if the given coordinates are within the bounds of the grid.
     * @return True if the coordinates are valid, false otherwise.
     */
    bool isValid(int x, int y) {
        return x >= 0 && x < GRID_WIDTH && y >= 0 && y < GRID_HEIGHT;
    }

    /**
     * Generates a random maze by initializing walls and setting the start and end points.
     * Also resets the distances and visited states of all cells.
     */
    void generateMaze() {
        std::random_device rd;
        std::mt19937 gen(rd());
        std::uniform_real_distribution<> dis(0, 1);

        // Initialize grid with walls and open spaces
        grid = std::vector<std::vector<Cell>>(GRID_WIDTH, std::vector<Cell>(GRID_HEIGHT));

        for (int i = 0; i < GRID_WIDTH; i++) {
            for (int j = 0; j < GRID_HEIGHT; j++) {
                grid[i][j].isWall = dis(gen) < 0.3; // 30% chance of being a wall
            }
        }

        // Define start and end points, ensuring they are not walls
        start = { 1, 1 };
        end = { GRID_WIDTH - 2, GRID_HEIGHT - 2 };
        grid[start.x][start.y].isWall = false;
        grid[end.x][end.y].isWall = false;

        // Reset distances and visited states
        for (int i = 0; i < GRID_WIDTH; i++) {
            for (int j = 0; j < GRID_HEIGHT; j++) {
                grid[i][j].visited = false;
                grid[i][j].distance = std::numeric_limits<int>::max();
            }
        }
        grid[start.x][start.y].distance = 0; // Start cell distance is 0
    }

    /**
     * Draws a single cell on the grid with colors indicating its state.
     */
    void drawCell(int x, int y) {
        sf::RectangleShape cell(sf::Vector2f(CELL_SIZE - 1, CELL_SIZE - 1));
        cell.setPosition(x * CELL_SIZE, y * CELL_SIZE);

        if (x == start.x && y == start.y) {
            cell.setFillColor(sf::Color::Green); // Start cell color
        }
        else if (x == end.x && y == end.y) {
            cell.setFillColor(sf::Color::Red); // End cell color
        }
        else if (grid[x][y].isWall) {
            cell.setFillColor(sf::Color::Black); // Wall color
        }
        else if (grid[x][y].visited) {
            cell.setFillColor(sf::Color(100, 100, 255)); // Visited cell color
        }
        else {
            cell.setFillColor(sf::Color::White); // Default cell color
        }

        window.draw(cell);
    }

    /**
     * Draws the solution path from the start to the end point using the `previous` field of each cell.
     */
    void drawPath() {
        if (!grid[end.x][end.y].visited) return; // No path to draw if the end is not visited

        sf::Vector2i current = end;
        while (!(current.x == start.x && current.y == start.y)) {
            sf::RectangleShape pathCell(sf::Vector2f(CELL_SIZE - 1, CELL_SIZE - 1));
            pathCell.setPosition(current.x * CELL_SIZE, current.y * CELL_SIZE);
            pathCell.setFillColor(sf::Color::Yellow); // Path color
            window.draw(pathCell);
            current = grid[current.x][current.y].previous; // Trace back the path
        }
    }

    /**
     * Executes one step of the Bellman-Ford algorithm to relax edges and update distances.
     * @return True if the grid is updated, false otherwise.
     */
    bool solveBellmanFordStep() {
        bool updated = false;

        // Relax all edges in the grid
        for (int x = 0; x < GRID_WIDTH; x++) {
            for (int y = 0; y < GRID_HEIGHT; y++) {
                if (grid[x][y].distance == std::numeric_limits<int>::max() || grid[x][y].isWall) {
                    continue; // Skip walls or uninitialized cells
                }

                // Directions for movement (up, down, left, right)
                const int dx[4] = { -1, 1, 0, 0 };
                const int dy[4] = { 0, 0, -1, 1 };

                for (int i = 0; i < 4; i++) {
                    int newX = x + dx[i];
                    int newY = y + dy[i];

                    if (isValid(newX, newY) && !grid[newX][newY].isWall) {
                        int newDist = grid[x][y].distance + 1;
                        if (newDist < grid[newX][newY].distance) {
                            grid[newX][newY].distance = newDist;
                            grid[newX][newY].previous = { x, y };
                            updated = true;
                        }
                    }
                }
            }
        }

        // Mark all visited nodes
        for (int x = 0; x < GRID_WIDTH; x++) {
            for (int y = 0; y < GRID_HEIGHT; y++) {
                if (grid[x][y].distance < std::numeric_limits<int>::max()) {
                    grid[x][y].visited = true;
                }
            }
        }

        // Return true if solving is incomplete
        return updated && grid[end.x][end.y].distance == std::numeric_limits<int>::max();
    }

public:
    /**
     * Constructor initializes the SFML window and generates the maze.
     */
    MazeSolver() : window(sf::VideoMode(GRID_WIDTH* CELL_SIZE, GRID_HEIGHT* CELL_SIZE), "Maze Solver") {
        generateMaze();
        solving = true;
    }

    /**
     * Main loop to run the maze solver. Handles events, updates the solver,
     * and renders the grid and solution.
     */
    void run() {
        while (window.isOpen()) {
            sf::Event event;
            while (window.pollEvent(event)) {
                if (event.type == sf::Event::Closed)
                    window.close();
                else if (event.type == sf::Event::KeyPressed) {
                    if (event.key.code == sf::Keyboard::R) {
                        // Reset maze
                        generateMaze();
                        solving = true;
                    }
                }
            }

            // Solve the maze step by step
            if (solving && clock.getElapsedTime().asSeconds() >= SOLVE_SPEED) {
                if (!solveBellmanFordStep()) {
                    solving = false;
                }
                clock.restart();
            }

            // Render the grid and path
            window.clear(sf::Color::White);
            for (int i = 0; i < GRID_WIDTH; i++) {
                for (int j = 0; j < GRID_HEIGHT; j++) {
                    drawCell(i, j);
                }
            }
            if (!solving) {
                drawPath();
            }
            window.display();
        }
    }
};
int main() {
    MazeSolver maze;
    maze.run();
    return 0;
}
